using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;

namespace WyprostujSie
{
    class Notifications
    {
        protected short actCount;
        private List<Tuple<string, Brush, string>> notificationsList;

        public void addNotif(string Content, Brush Background, string Tag)
        {
            Tuple<string, Brush, string> newNotif = new Tuple<string, Brush, string>(Content, Background, Tag);
            notificationsList.Add(newNotif);
        }

        public void delNotif(string Tag)
        {
           
        }

        public void updNotif(string Content, Brush Background, string Tag)
        {

        }

        public Notifications()
        {
            notificationsList = new List<Tuple<string, Brush, string>>();
            actCount = 0;
        }

        public Tuple<string, Brush> ChNoti()
        {
            if (notificationsList == null)
            {
                return new Tuple<string, Brush>("", Brushes.Transparent);
            }
            else
            {
                actCount++;
                if (actCount > notificationsList.Count-1)
                    actCount = 0;
                return new Tuple<string, Brush>(notificationsList[actCount].Item1, notificationsList[actCount].Item2);
            }
        }
    }
}
