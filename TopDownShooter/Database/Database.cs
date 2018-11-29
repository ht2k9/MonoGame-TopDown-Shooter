using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;

namespace Database
{
    public class Data
    {
        // Create a Fighter object and serialize it to a JSON stream.  
        public static List<Fighter> ReadObject(string fileName)
        {
            if (!File.Exists(fileName))
                return new List<Fighter>();

            String JSONstring = File.ReadAllText(fileName);

            //Fighter fighter = JsonConvert.DeserializeObject<Fighter>(JSONstring);
            List<Fighter> fighters = JsonConvert.DeserializeObject<List<Fighter>>(JSONstring);

            if (fighters == null)
                fighters = new List<Fighter>();

            return fighters;
        }

        // Deserialize a JSON stream to a Fighter object.  
        public static void WriteToObject(string json, List<Fighter> data)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();

            string outputJSON = ser.Serialize(data);

            File.WriteAllText(json, outputJSON);
        }

        public static void SetScore(int score)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();

            string outputJSON = ser.Serialize(score);

            File.WriteAllText("score.json", outputJSON);
        }

        public static int GetScore()
        {
            String JSONstring = File.ReadAllText("score.json");

            int score = JsonConvert.DeserializeObject<int>(JSONstring);

            return score;
        }

        public static void SetZombie(string[] score)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();

            string outputJSON = ser.Serialize(score);

            File.WriteAllText("zombie.json", outputJSON);
        }

        public static string[] GetZombie()
        {
            String JSONstring = File.ReadAllText("zombie.json");

            string[] score = JsonConvert.DeserializeObject<string[]>(JSONstring);

            return score;
        }
    }

    public class Fighter : Data
    {
        public string name { get; set; }
        public string weapontype { get; set; }
        public float firerate { get; set; }
        public double health { get; set; }
        public double damage { get; set; }
        public int magazineSize { get; set; }
        public bool canUse { get; set; }

        public Fighter(string name, string weapontype, float firerate, double health, double damage, int magazineSize, bool canUse)
        {
            this.name = name;
            this.weapontype = weapontype;
            this.firerate = firerate;
            this.magazineSize = magazineSize;
            this.health = health;
            this.damage = damage;
            this.canUse = canUse;
        }

        public void CanUse(string name)
        {
            if (weapontype == name.ToLower())
                canUse = true;
        }

        public override string ToString()
        {
            return "name: " + name + "\n" +
                "weapon type: " + weapontype;
        }
    }
}


