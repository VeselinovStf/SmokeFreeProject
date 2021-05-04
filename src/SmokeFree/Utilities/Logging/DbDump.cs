using Realms;
using SmokeFree.Data.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SmokeFree.Utilities.Logging
{
    public static class DbDump
    {
        public static string[] DumpInMemory(Realm realm)
        {

            List<string> dataRows = new List<string>();

            var users = realm.All<User>().ToList();
            var challenges = realm.All<Challenge>().ToList();
            var challengeResults = realm.All<ChallengeResult>().ToList();
            var challengeSmokes = realm.All<ChallengeSmoke>().ToList();
            var dayChallengeSmokes = realm.All<DayChallengeSmoke>().ToList();
            var smokes = realm.All<Smoke>().ToList();
            var test = realm.All<Test>().ToList();
            var testResults = realm.All<TestResult>().ToList();

            dataRows.Add("Users");
            dataRows.AddRange(GetFromEntity(users));
            dataRows.Add("");

            dataRows.Add("challenges");
            dataRows.AddRange(GetFromEntity(challenges));
            dataRows.Add("");

            dataRows.Add("challengeResults");
            dataRows.AddRange(GetFromEntity(challengeResults));
            dataRows.Add("");

            dataRows.Add("challengeSmokes");
            dataRows.AddRange(GetFromEntity(challengeSmokes));
            dataRows.Add("");

            dataRows.Add("dayChallengeSmokes");
            dataRows.AddRange(GetFromEntity(dayChallengeSmokes));
            dataRows.Add("");

            dataRows.Add("test");
            dataRows.AddRange(GetFromEntity(test));
            dataRows.Add("");

            dataRows.Add("smokes");
            dataRows.AddRange(GetFromEntity(smokes));
            dataRows.Add("");

            dataRows.Add("testResults");
            dataRows.AddRange(GetFromEntity(testResults));
            dataRows.Add("");


            return dataRows.ToArray();
        }

        private static IEnumerable<string>  GetFromEntity<T>(List<T> entity)
        {
            List<string> res = new List<string>();

            StringBuilder builder = new StringBuilder();
            
            var headerT = entity.FirstOrDefault();
            Type headerType = headerT.GetType();
            PropertyInfo[] headerProps = headerType.GetProperties();

            foreach (var prop in headerProps)
            {
                if (prop.GetIndexParameters().Length == 0)
                {
                    builder.Append(prop.Name.ToString() + ",");
                }
            }

            res.Add(builder.ToString().TrimEnd(','));
            builder.Clear();

            foreach (var e in entity)
            {
                try
                {
                    object obj = e;
                    Type t = e.GetType();
                    PropertyInfo[] props = t.GetProperties();

                    foreach (var prop in props)
                    {
                        if (prop.GetIndexParameters().Length == 0)
                        {                           
                            builder.Append(prop.GetValue(obj).ToString() + ",");
                        }                          
                    }

                    res.Add(builder.ToString().TrimEnd(','));
                    builder.Clear();


                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }              
            }
            
            return res;
        }
    }
}
