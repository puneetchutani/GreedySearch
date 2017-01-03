using GreedyMatch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace GreedyMatch.Controllers
{
    public class GreedyMatchController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Home()
        {
            return View();
        }

        //TO CONCATENATE TWO STRING
        public static string StringMerge(string firstString, string secondString)
        {
            if (string.IsNullOrEmpty(firstString)) return secondString;
            if (string.IsNullOrEmpty(secondString)) return firstString;
            if (firstString.Contains(secondString)) return firstString;
            if (secondString.Contains(firstString)) return secondString;    

            char endChar = firstString.ToCharArray().Last();
            char startChar = secondString.ToCharArray().First();

            int firstStringFirstIndexOfStartChar = firstString.IndexOf(startChar);
            int overlapStringLength = firstString.Length - firstStringFirstIndexOfStartChar;

            while (overlapStringLength >= 0 && firstStringFirstIndexOfStartChar >= 0)
            {
                if (CheckOverlapping(firstString, secondString, overlapStringLength))
                {
                    return firstString + secondString.Substring(overlapStringLength);
                }

                firstStringFirstIndexOfStartChar =
                    firstString.IndexOf(startChar, firstStringFirstIndexOfStartChar + 1);
                overlapStringLength = firstString.Length - firstStringFirstIndexOfStartChar;

            }

            return firstString + secondString;
        }

        //TO CHECK TWO OVERLAPPED STRINGS IF ANY
        private static bool CheckOverlapping(string firstString, string secondString, int overlapStringLength)
        {
            if (overlapStringLength <= 0)
                return false;

            if (secondString.Length < overlapStringLength)
                return false;

            if (firstString.Substring(firstString.Length - overlapStringLength) == secondString.Substring(0, overlapStringLength))
                return true;

            return false;
        }

        //MERGE TWO STRINGS AT A TIME
        public string patternMatching(string[] words) {
            string result = "";
            string first = words[0];
            string second = words[1];
            result += StringMerge(first, second);

            for (int i = 2; i < words.Length - 1; i++)
                result = StringMerge(result, words[i]);

            return result;
        }

        // VALIDATION PERFORMED AND REASSEMBLING THE STRING
        [HttpPost]
        public JsonResult CheckValidText(GreedyMatchModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, errorMessage = "Value cannot be left blank or fragment length is exceeded beyond limit" }, JsonRequestBehavior.AllowGet);

            if (model.Fragments == 0)
                return Json(new { success = true, data = "" }, JsonRequestBehavior.AllowGet);

            if (model.Fragments < 0 || model.Fragments > 5000)
                return Json(new { success = false, errorMessage = "Fragment length cannot be less than 0 and greater than 5000" }, JsonRequestBehavior.AllowGet);

            if (string.IsNullOrEmpty(model.StrText))
                return Json(new { success = true, data = model.StrText }, JsonRequestBehavior.AllowGet);

            var strText = model.StrText.Trim();

            if (!strText.StartsWith("{") || !strText.EndsWith("}"))
                return Json(new { success = false, errorMessage = "Text must be enclosed in curly braces" }, JsonRequestBehavior.AllowGet);

            string[] words = strText.Split('}');

            for (int i = 0; i < words.Length - 1; i++)
            {
                if (words[i].IndexOf('{') == -1)
                    return Json(new { success = false, errorMessage = "Wrong input format" }, JsonRequestBehavior.AllowGet);

                words[i] = words[i].Substring(words[i].IndexOf('{') + 1).Trim();

                //REPLACE ALL NEW LINE CHARACTER WITH SPACES
                words[i] = Regex.Replace(words[i], @"\r\n?|\n", String.Empty);

                if (words[i].Contains('{'))
                    return Json(new { success = false, errorMessage = "Wrong input format" }, JsonRequestBehavior.AllowGet);

                if (words[i].Length > 1000)
                    return Json(new { success = false, errorMessage = "The maximum length of the fragmented string is 1000" }, JsonRequestBehavior.AllowGet);

            }
            if (words.Length - 1 != model.Fragments)
                return Json(new { success = false, errorMessage = "Fragments does not equal to number of strings" }, JsonRequestBehavior.AllowGet);

            if (words.Length <= 2)
                return Json(new { success = true, data = words[0] }, JsonRequestBehavior.AllowGet);

            var result = patternMatching(words);
            return Json(new { success = true, data = result }, JsonRequestBehavior.AllowGet);
        }
    }
}