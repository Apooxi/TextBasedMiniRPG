using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace TextBasedMiniRPG
{
    internal class DatabaseManager
    {
        private readonly string connString = "Host=localhost;Username=postgres;Password=admin;Database=ArenaDB";

        public void AddCharacter(string type, string name, int health, int damage, int mana)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                string sqlQuery = $"INSERT INTO Characters (CharacterType, Name, Health, Damage, Mana, Wins) VALUES ('{type}', '{name}', {health}, {damage}, {mana}, 0)";

                using (NpgsqlCommand cmd = new NpgsqlCommand(sqlQuery, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            Console.WriteLine($"\n[Sistem] {name} isimli {type} başarıyla veritabanına kaydedildi!");
        }

        public List<Character> GetAllCharacters()
        {
            List<Character> roster = new List<Character>();

            using(NpgsqlConnection conn = new NpgsqlConnection( connString))
            {
                conn.Open();
                string sqlQuery = "SELECT Id, CharacterType, Name, Health, Damage, Mana, Level, XP FROM Characters";

                using(NpgsqlCommand cmd = new NpgsqlCommand(sqlQuery, conn))
                using(NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        string type = reader["CharacterType"].ToString();
                        string name = reader["Name"].ToString();
                        int hp = Convert.ToInt32(reader["Health"]);
                        int dmg = Convert.ToInt32(reader["Damage"]);
                        int mana = Convert.ToInt32(reader["Mana"]);
                        int level = Convert.ToInt32(reader["Level"]);
                        int xp = Convert.ToInt32(reader["XP"]);

                        Character newChar;
                        if (type == "Warrior")
                        {
                            newChar = new Warrior(name, hp, dmg, mana);
                        }
                        else
                        {
                            newChar = new Wizard(name, hp, dmg, mana);
                        }

                        newChar.Level = level;
                        newChar.Xp = xp;

                        roster.Add(newChar);
                    }
                }
            }
            return roster;
        }

        public void AddWinToCharacters(string characterName)
        {
            using(NpgsqlConnection conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                string sqlQuery = $"UPDATE Characters SET Wins = Wins + 1 WHERE Name = '{characterName}'";

                using(NpgsqlCommand cmd = new NpgsqlCommand( sqlQuery, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateCharacterStats(Character character)
        {
            using(NpgsqlConnection conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                string sqlQuery = $"UPDATE Characters SET Level = {character.Level}, XP = {character.Xp}, Health = {character.Health}, Damage = {character.Damage}, Mana = {character.Mana} WHERE Name = '{character.Name}'";

                using(NpgsqlCommand cmd = new NpgsqlCommand(sqlQuery, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
