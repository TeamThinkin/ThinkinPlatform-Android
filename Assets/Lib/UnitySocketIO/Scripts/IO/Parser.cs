using UnityEngine;
using System.Collections;
using UnitySocketIO.Events;

namespace UnitySocketIO.IO
{
    public class Parser
    {
        //I rewrote the parsing code because the original did not properly account for messages with no data and would parse the event name incorrectly - mbell 8/8/21
        public SocketIOEvent Parse(string json)
        {
            // Example data:
            // ["show_story",{ "key":"asdf","session":""}]
            // ["CMS_UPDATE"]

            var eventName = json.Substring(2, json.IndexOf('"', 2) - 2); //Find the second quote that denotes the end of the event name
            var commaIndex = json.IndexOf(',');

            if (commaIndex == -1)
            {
                return new SocketIOEvent(eventName);
            }
            else
            {
                var data = json.Substring(commaIndex + 1, json.Length - commaIndex - 2);
                return new SocketIOEvent(eventName, data);
            }
        }

        //Original flawed parsing code
        //public SocketIOEvent Parse(string json)
        //{
        //    string[] data = json.Split(new char[] { ',' }, 2);
        //    string e = data[0].Substring(2, data[0].Length - (data.Length > 1 ? 3 : 2));

        //    if(data.Length == 1)
        //    {
        //        return new SocketIOEvent(e);
        //    }

        //    return new SocketIOEvent(e, data[1].TrimEnd(']'));
        //}

        public string ParseData(string json) {
            return json.Substring(1, json.Length - 2);
        }
    }
}