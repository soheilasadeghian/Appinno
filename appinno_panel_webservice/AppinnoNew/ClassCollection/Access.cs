using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace AppinnoNew.ClassCollection
{
    public class Accessing
    {
        public bool relation { get; set; }
        public News news { get; set; }
        public Event events { get; set; }
        public IO io { get; set; }
        public Publication publication { get; set; }
        public Chart chart { get; set; }
        public Download download { get; set; }
        public Grouping grouping { get; set; }
        public User user { get; set; }
        public Access access { get; set; }
        public Operator operators { get; set; }
        public Ican ican { get; set; }
        public Poll poll { get; set; }
        public BestIdeasCompetition bestIdeasCompetition { get; set; }
        public CreativityCompetition creativityCompetition { get; set; }
        public MyIranCompetition myIranCompetition { get; set; }

        public static Accessing Deserialize(string encodedAccess)
        {
            encodedAccess = ClassCollection.Methods.Decrypt(encodedAccess);
            return new JavaScriptSerializer().Deserialize<ClassCollection.Accessing>(encodedAccess);
        }
        public string ToString(bool encode)
        {
            string result = "";

            result = new JavaScriptSerializer().Serialize(this);

            if (encode)
                return ClassCollection.Methods.Encrypt(result);
            else
                return result;
        }

        #region class Definition
        public class News
        {
            public AccessDetail access { get; set; }
            [ScriptIgnore]
            public bool isFull
            {
                get
                {

                    return access.delete && access.edit && access.insert && access.showlist;

                }
            }
        }
        public class Event
        {
            public AccessDetail access { get; set; }
            [ScriptIgnore]
            public bool isFull
            {
                get
                {

                    return access.delete && access.edit && access.insert && access.showlist;

                }
            }
        }
        public class IO
        {
            public AccessDetail access { get; set; }
            [ScriptIgnore]
            public bool isFull
            {
                get
                {

                    return access.delete && access.edit && access.insert && access.showlist;

                }
            }
        }
        public class Publication
        {
            public AccessDetail access { get; set; }
            [ScriptIgnore]
            public bool isFull
            {
                get
                {

                    return access.delete && access.edit && access.insert && access.showlist;

                }
            }
        }
        public class Chart
        {
            public AccessDetail access { get; set; }
            [ScriptIgnore]
            public bool isFull
            {
                get
                {

                    return access.delete && access.edit && access.insert && access.showlist;

                }
            }
        }
        public class Download
        {
            public AccessDetail access { get; set; }
            [ScriptIgnore]
            public bool isFull
            {
                get
                {

                    return access.delete && access.edit && access.insert && access.showlist;

                }
            }
        }
        public class Ican
        {
            public AccessDetail access { get; set; }
            [ScriptIgnore]
            public bool isFull
            {
                get
                {

                    return access.delete && access.edit && access.insert && access.showlist;

                }
            }
        }
        public class Grouping
        {
            public AccessDetail access { get; set; }
            [ScriptIgnore]
            public bool isFull
            {
                get
                {

                    return access.delete && access.edit && access.insert && access.showlist;

                }
            }
        }
        public class User
        {
            public AccessDetail access { get; set; }
            [ScriptIgnore]
            public bool isFull
            {
                get
                {

                    return access.delete && access.edit && access.insert && access.showlist;

                }
            }
        }
        public class Access
        {
            public AccessDetail access { get; set; }
            [ScriptIgnore]
            public bool isFull
            {
                get
                {

                    return access.delete && access.edit && access.insert && access.showlist;

                }
            }
        }
        public class Operator
        {
            public AccessDetail access { get; set; }
            [ScriptIgnore]
            public bool isFull
            {
                get
                {

                    return access.delete && access.edit && access.insert && access.showlist;

                }
            }
        }

        public class Poll
        {
            public AccessDetail access { get; set; }
            [ScriptIgnore]
            public bool isFull
            {
                get
                {

                    return access.delete && access.edit && access.insert && access.showlist;

                }
            }
        }
        public class BestIdeasCompetition
        {
            public AccessDetail access { get; set; }
            [ScriptIgnore]
            public bool isFull
            {
                get
                {

                    return access.delete && access.edit && access.insert && access.showlist;

                }
            }
        }
        public class CreativityCompetition
        {

            public AccessDetail access { get; set; }
            [ScriptIgnore]
            public bool isFull
            {
                get
                {

                    return access.delete && access.edit && access.insert && access.showlist;

                }
            }
        }
        public class MyIranCompetition
        {

            public AccessDetail access { get; set; }
            [ScriptIgnore]
            public bool isFull
            {
                get
                {

                    return access.delete && access.edit && access.insert && access.showlist;

                }
            }
        }

        public class AccessDetail
        {
            private bool _showlist, _edit, _delete, _insert;
            public bool showlist
            {
                get { return _showlist; }
                set
                {
                    if (!value)
                    {
                        _insert = _edit = _delete = false;
                    }

                    _showlist = value;
                }
            }
            public bool insert
            {
                get { return _insert; }
                set
                {
                    if (value)
                    {
                        _showlist = true;
                    }
                    _insert = value;
                }
            }
            public bool edit
            {
                get { return _edit; }
                set
                {
                    if (value)
                    {
                        _showlist = true;
                    }
                    _edit = value;
                }
            }
            public bool delete
            {
                get { return _delete; }
                set
                {
                    if (value)
                    {
                        _showlist = true;
                    }
                    _delete = value;
                }
            }

            //public AccessDetail()
            //{ }
            //public AccessDetail(string access, bool encoded)
            //{
            //    if (encoded)
            //        access = ClassCollection.Methods.Decrypt(access);

            //    var parts = access.Split('/');

            //    showList = parts.Any(c => c.Contains("showlist"));
            //    insert = parts.Any(c => c.Contains("insert"));
            //    edit = parts.Any(c => c.Contains("edit"));
            //    delete = parts.Any(c => c.Contains("delete"));
            //}
            //public void setAccess(string access, bool encoded)
            //{
            //    if (encoded)
            //        access = ClassCollection.Methods.Decrypt(access);

            //    var parts = access.Split('/');

            //    showList = parts.Any(c => c.Contains("showlist"));
            //    insert = parts.Any(c => c.Contains("insert"));
            //    edit = parts.Any(c => c.Contains("edit"));
            //    delete = parts.Any(c => c.Contains("delete"));
            //}
            //public string getAccess(bool encode)
            //{
            //    string result = "";

            //    if (showList)
            //        result += "showlist";
            //    if (insert)
            //        result += "/insert";
            //    if (edit)
            //        result += "/edit";
            //    if (delete)
            //        result += "/delete";

            //    if (encode)
            //        return ClassCollection.Methods.Encrypt(result);
            //    else
            //        return result;
            //}
        }

        #endregion
    }
}