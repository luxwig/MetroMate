using System;
using System.Collections.Generic;
using UIKit;


namespace MetroMate
{
    public class FeedMessageException : Exception
    {
        public FeedMessageException() : base() { }
        public FeedMessageException(Exception e) : base(e.Message, e.InnerException) { }
    }

    public class FeedFetchException : Exception
    {

        private List<string> LineStr;
        private List<string> ExceptionStr;
        public override string Message
        {
            get
            {
                string s = "Invalid feed occur at:";
                for (int i = 0; i < LineStr.Count; i++)
                    s += ("\n" + LineStr[i] + "\t" + ExceptionStr[i]);
                return s;
            }
        }

        public FeedFetchException() : base()
        {
            LineStr = new List<string>();
            ExceptionStr = new List<string>();
        }
        
        public void AddException(Exception e, string LineStr)
        {
            if (this.LineStr.Contains(LineStr))
                return;
            this.LineStr.Add(LineStr);
            ExceptionStr.Add(e.Message);
        }

        public void ShowAlert(UIViewController v)
        {
            //Create Alert
            var okAlertController = UIAlertController.Create("ERROR", Message, UIAlertControllerStyle.Alert);

            //Add Action
            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));

            // Present Alert
            v.PresentViewController(okAlertController, true, null);
        }
    }
}
