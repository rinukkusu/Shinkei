using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Data;
using System;

namespace SandraPlugin
{
    public class Markov
    {
        private readonly MySqlConnection _conn;

        public Markov(string host, string db, string user, string pw)
        {
            _conn = new MySqlConnection("SERVER=" + host + ";" +
                                        "DATABASE=" + db + ";" +
                                        "UID=" + user + ";" +
                                        "PASSWORD=" + pw + ";");


        }

        public bool DatabaseAvailable()
        {
            try
            {
                _conn.Open();
                _conn.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void AddSentence(string text)
        {
            List<string> words = new List<string>(text.Split(' '));
            words.RemoveAll(string.IsNullOrWhiteSpace);

            for (int i = 0; i <= words.Count - 1; i++)
            {
                InsertPair(words[i], (i < words.Count - 1) ? words[i + 1] : "");
            }
        }

        public void InsertPair(string word1, string word2)
        {
            word1 = word1.ToLower();
            word2 = word2.ToLower();

            long wId1 = TryInsertWord(word1);
            long wId2 = 0;

            if ((word2.Length > 0))
            {
                wId2 = TryInsertWord(word2);
            }

            if ((_conn.State == ConnectionState.Open))
            {
                _conn.Close();
            }
            _conn.Open();
            MySqlCommand cmd = _conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM `markovparts` WHERE `word1`=" + wId1 + " AND `word2`=" + wId2 + ";";

            MySqlDataReader reader = cmd.ExecuteReader();
            if ((reader.HasRows == false))
            {
                _conn.Close();
                _conn.Open();
                cmd = _conn.CreateCommand();
                cmd.CommandText = "INSERT INTO `markovparts` SET `word1`=" + wId1 + ", `word2`=" + wId2 + ";";
                cmd.ExecuteNonQuery();
                _conn.Close();
            }
            else
            {
                _conn.Close();
                _conn.Open();
                cmd = _conn.CreateCommand();
                cmd.CommandText = "UPDATE `markovparts` SET `count`=`count`+1 WHERE `word1`=" + wId1 + " AND `word2`=" +
                                  wId2 + ";";
                cmd.ExecuteNonQuery();
                _conn.Close();
            }

        }

        public int GetWordId(string word)
        {
            int id = -1;

            if ((_conn.State == ConnectionState.Open))
            {
                _conn.Close();
            }

            _conn.Open();
            MySqlCommand cmd = _conn.CreateCommand();
            cmd.CommandText = "SELECT id FROM `words` WHERE `word`='" + word + "';";

            MySqlDataReader reader = cmd.ExecuteReader();

            if ((reader.HasRows))
            {
                reader.Read();
                id = reader.GetInt32("id");
            }

            _conn.Close();

            return id;
        }

        public string GetWordById(long id)
        {
            string word = string.Empty;

            if ((_conn.State == ConnectionState.Open))
            {
                _conn.Close();
            }
            _conn.Open();
            MySqlCommand cmd = _conn.CreateCommand();
            cmd.CommandText = "SELECT word FROM `words` WHERE `id`=" + id + ";";

            MySqlDataReader reader = cmd.ExecuteReader();

            if ((reader.HasRows))
            {
                reader.Read();
                word = reader.GetString("word");
            }

            _conn.Close();

            return word;
        }

        public long TryInsertWord(string word)
        {
            long id = GetWordId(word);

            if ((_conn.State == ConnectionState.Open))
            {
                _conn.Close();
            }
            _conn.Open();
            if ((id < 0))
            {
                MySqlCommand cmd = _conn.CreateCommand();
                cmd.CommandText = "INSERT INTO `words` SET `word`='" + word + "';";
                cmd.ExecuteNonQuery();
                id = cmd.LastInsertedId;
            }
            _conn.Close();

            return id;
        }

        public string GetSentence(string word1 = "ich")
        {
            string text;

            long startId = GetWordId(word1);
            if ((startId > 0))
            {
                text = word1;
                long nextId = GetNextWord(startId);
                while ((nextId > 0))
                {
                    text += " " + GetWordById(nextId);
                    nextId = GetNextWord(nextId);
                }
            }
            else
            {
                return null;
            }

            return text;
        }

        public long GetNextWord(long id)
        {
            long nextid = 0;

            if ((_conn.State == ConnectionState.Open))
            {
                _conn.Close();
            }
            _conn.Open();
            MySqlCommand cmd = _conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM `markovparts` WHERE `word1`=" + id + " ORDER BY RAND() LIMIT 1;";

            MySqlDataReader reader = cmd.ExecuteReader();
            if ((reader.HasRows))
            {
                reader.Read();

                nextid = reader.GetInt64("word2");
            }

            _conn.Close();

            return nextid;
        }
    }
}