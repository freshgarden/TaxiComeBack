using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Microsoft.Security.Application;

namespace TaxiCameBack.Core.Utilities
{
    public static class StringUtils
    {
        /// <summary>
        /// Returns safe plain text using XSS library
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string SafePlainText(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                input = StripHtmlFromString(input);
                input = GetSafeHtml(input, true);
            }
            return input;
        }

        /// <summary>
        /// Used to pass all string input in the system  - Strips all nasties from a string/html
        /// </summary>
        /// <param name="html"></param>
        /// <param name="useXssSantiser"></param>
        /// <returns></returns>
        public static string GetSafeHtml(string html, bool useXssSantiser = false)
        {
            // Scrub html
            html = ScrubHtml(html, useXssSantiser);

            // remove unwanted html
            html = RemoveUnwantedTags(html);

            return html;
        }

        /// <summary>
        /// Create a salt for the password hash (just makes it a bit more complex)
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string CreateSalt(int size)
        {
            // Generate a cryptographic random number.
            var rng = new RNGCryptoServiceProvider();
            var buff = new byte[size];
            rng.GetBytes(buff);

            // Return a Base64 string representation of the random number.
            return Convert.ToBase64String(buff);
        }

        /// <summary>
        /// Generate a hash for a password, adding a salt value
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public static string GenerateSaltedHash(string plainText, string salt)
        {
            // http://stackoverflow.com/questions/2138429/hash-and-salt-passwords-in-c-sharp

            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            var saltBytes = Encoding.UTF8.GetBytes(salt);

            // Combine the two lists
            var plainTextWithSaltBytes = new List<byte>(plainTextBytes.Length + saltBytes.Length);
            plainTextWithSaltBytes.AddRange(plainTextBytes);
            plainTextWithSaltBytes.AddRange(saltBytes);

            // Produce 256-bit hashed value i.e. 32 bytes
            HashAlgorithm algorithm = new SHA256Managed();
            var byteHash = algorithm.ComputeHash(plainTextWithSaltBytes.ToArray());
            return Convert.ToBase64String(byteHash);
        }


        /// <summary>
        /// Takes in HTML and returns santized Html/string
        /// </summary>
        /// <param name="html"></param>
        /// <param name="useXssSantiser"></param>
        /// <returns></returns>
        public static string ScrubHtml(string html, bool useXssSantiser = false)
        {
            if (string.IsNullOrEmpty(html))
            {
                return html;
            }

            // clear the flags on P so unclosed elements in P will be auto closed.
            HtmlNode.ElementsFlags.Remove("p");

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var finishedHtml = html;

            // Embed Urls
            if (doc.DocumentNode != null)
            {
                // Get all the links we are going to 
                var tags = doc.DocumentNode.SelectNodes("//a[contains(@href, 'youtube.com')]|//a[contains(@href, 'youtu.be')]|//a[contains(@href, 'vimeo.com')]|//a[contains(@href, 'screenr.com')]|//a[contains(@href, 'instagram.com')]");

                if (tags != null)
                {
                    // find formatting tags
                    foreach (var item in tags)
                    {
                        if (item.PreviousSibling == null)
                        {
                            // Prepend children to parent node in reverse order
                            foreach (var node in item.ChildNodes.Reverse())
                            {
                                item.ParentNode.PrependChild(node);
                            }
                        }
                        else
                        {
                            // Insert children after previous sibling
                            foreach (var node in item.ChildNodes)
                            {
                                item.ParentNode.InsertAfter(node, item.PreviousSibling);
                            }
                        }

                        // remove from tree
                        item.Remove();
                    }
                }


                //Remove potentially harmful elements
                var nc = doc.DocumentNode.SelectNodes("//script|//link|//iframe|//frameset|//frame|//applet|//object|//embed");
                if (nc != null)
                {
                    foreach (var node in nc)
                    {
                        node.ParentNode.RemoveChild(node, false);

                    }
                }

                //remove hrefs to java/j/vbscript URLs
                nc = doc.DocumentNode.SelectNodes("//a[starts-with(translate(@href, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'javascript')]|//a[starts-with(translate(@href, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'jscript')]|//a[starts-with(translate(@href, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'vbscript')]");
                if (nc != null)
                {

                    foreach (var node in nc)
                    {
                        node.SetAttributeValue("href", "#");
                    }
                }

                //remove img with refs to java/j/vbscript URLs
                nc = doc.DocumentNode.SelectNodes("//img[starts-with(translate(@src, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'javascript')]|//img[starts-with(translate(@src, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'jscript')]|//img[starts-with(translate(@src, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'vbscript')]");
                if (nc != null)
                {
                    foreach (var node in nc)
                    {
                        node.SetAttributeValue("src", "#");
                    }
                }

                //remove on<Event> handlers from all tags
                nc = doc.DocumentNode.SelectNodes("//*[@onclick or @onmouseover or @onfocus or @onblur or @onmouseout or @ondblclick or @onload or @onunload or @onerror]");
                if (nc != null)
                {
                    foreach (var node in nc)
                    {
                        node.Attributes.Remove("onFocus");
                        node.Attributes.Remove("onBlur");
                        node.Attributes.Remove("onClick");
                        node.Attributes.Remove("onMouseOver");
                        node.Attributes.Remove("onMouseOut");
                        node.Attributes.Remove("onDblClick");
                        node.Attributes.Remove("onLoad");
                        node.Attributes.Remove("onUnload");
                        node.Attributes.Remove("onError");
                    }
                }

                // remove any style attributes that contain the word expression (IE evaluates this as script)
                nc = doc.DocumentNode.SelectNodes("//*[contains(translate(@style, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'), 'expression')]");
                if (nc != null)
                {
                    foreach (var node in nc)
                    {
                        node.Attributes.Remove("stYle");
                    }
                }

                // build a list of nodes ordered by stream position
                var pos = new NodePositions(doc);

                // browse all tags detected as not opened
                foreach (var error in doc.ParseErrors.Where(e => e.Code == HtmlParseErrorCode.TagNotOpened))
                {
                    // find the text node just before this error
                    var last = pos.Nodes.OfType<HtmlTextNode>().LastOrDefault(n => n.StreamPosition < error.StreamPosition);
                    if (last != null)
                    {
                        // fix the text; reintroduce the broken tag
                        last.Text = error.SourceText.Replace("/", "") + last.Text + error.SourceText;
                    }
                }

                finishedHtml = doc.DocumentNode.WriteTo();
            }


            // The reason we have this option, is using the santiser with the MarkDown editor 
            // causes problems with line breaks.
            if (useXssSantiser)
            {
                return SanitizerCompatibleWithForiegnCharacters(Sanitizer.GetSafeHtmlFragment(finishedHtml));
            }

            return finishedHtml;
        }

        public static string RemoveUnwantedTags(string html)
        {

            var unwantedTagNames = new List<string>
            {
                "div",
                "font",
                "table",
                "tbody",
                "tr",
                "td",
                "th",
                "thead"
            };

            return RemoveUnwantedTags(html, unwantedTagNames);
        }

        public static string RemoveUnwantedTags(string html, List<string> unwantedTagNames)
        {
            if (string.IsNullOrEmpty(html))
            {
                return html;
            }

            var htmlDoc = new HtmlDocument();

            // load html
            htmlDoc.LoadHtml(html);

            var tags = (from tag in htmlDoc.DocumentNode.Descendants()
                        where unwantedTagNames.Contains(tag.Name)
                        select tag).Reverse();


            // find formatting tags
            foreach (var item in tags)
            {
                if (item.PreviousSibling == null)
                {
                    // Prepend children to parent node in reverse order
                    foreach (var node in item.ChildNodes.Reverse())
                    {
                        item.ParentNode.PrependChild(node);
                    }
                }
                else
                {
                    // Insert children after previous sibling
                    foreach (var node in item.ChildNodes)
                    {
                        item.ParentNode.InsertAfter(node, item.PreviousSibling);
                    }
                }

                // remove from tree
                item.Remove();
            }

            // return transformed doc
            return htmlDoc.DocumentNode.WriteContentTo().Trim();
        }

        /// <summary>
        /// Uses regex to strip HTML from a string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string StripHtmlFromString(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                input = Regex.Replace(input, @"</?\w+((\s+\w+(\s*=\s*(?:"".*?""|'.*?'|[^'"">\s]+))?)+\s*|\s*)/?>", string.Empty, RegexOptions.Singleline);
                input = Regex.Replace(input, @"\[[^]]+\]", "");
            }
            return input;
        }

        private static readonly Dictionary<string, string> HbjDictionaryFx = new Dictionary<string, string>();

        /// <summary>
        /// 微软的AntiXSS v4.0 让部分汉字乱码,这里将乱码部分汉字转换回来
        /// Microsoft AntiXSS Library Sanitizer causes some Chinese characters become "encoded",
        /// use this function to replace them back.
        /// source:http://www.zhaoshu.net/bbs/read10.aspx?TieID=b1745a9c-03a6-4367-93e0-114707aff3e3
        /// </summary>
        /// <returns></returns>
        public static string SanitizerCompatibleWithForiegnCharacters(string originalString)
        {
            var returnString = originalString;

            //returnString = returnString.Replace("\r\n", "");
            if (returnString.Contains("&#"))
            {
                //Initialize the dictionary, if it doesn't contain anything. 
                if (HbjDictionaryFx.Keys.Count == 0)
                {
                    lock (HbjDictionaryFx)
                    {
                        if (HbjDictionaryFx.Keys.Count == 0)
                        {
                            HbjDictionaryFx.Clear();
                            HbjDictionaryFx.Add("&#20028;", "丼");
                            HbjDictionaryFx.Add("&#20284;", "似");
                            HbjDictionaryFx.Add("&#20540;", "值");
                            HbjDictionaryFx.Add("&#20796;", "儼");
                            HbjDictionaryFx.Add("&#21052;", "刼");
                            HbjDictionaryFx.Add("&#21308;", "匼");
                            HbjDictionaryFx.Add("&#21564;", "吼");
                            HbjDictionaryFx.Add("&#21820;", "唼");
                            HbjDictionaryFx.Add("&#22076;", "嘼");
                            HbjDictionaryFx.Add("&#22332;", "圼");
                            HbjDictionaryFx.Add("&#22588;", "堼");
                            HbjDictionaryFx.Add("&#23612;", "尼");
                            HbjDictionaryFx.Add("&#26684;", "格");
                            HbjDictionaryFx.Add("&#22844;", "夼");
                            HbjDictionaryFx.Add("&#23100;", "娼");
                            HbjDictionaryFx.Add("&#23356;", "嬼");
                            HbjDictionaryFx.Add("&#23868;", "崼");
                            HbjDictionaryFx.Add("&#24124;", "帼");
                            HbjDictionaryFx.Add("&#24380;", "弼");
                            HbjDictionaryFx.Add("&#24636;", "怼");
                            HbjDictionaryFx.Add("&#24892;", "愼");
                            HbjDictionaryFx.Add("&#25148;", "戼");
                            HbjDictionaryFx.Add("&#25404;", "挼");
                            HbjDictionaryFx.Add("&#25660;", "搼");
                            HbjDictionaryFx.Add("&#25916;", "攼");
                            HbjDictionaryFx.Add("&#26172;", "昼");
                            HbjDictionaryFx.Add("&#26428;", "朼");
                            HbjDictionaryFx.Add("&#26940;", "椼");
                            HbjDictionaryFx.Add("&#27196;", "樼");
                            HbjDictionaryFx.Add("&#27452;", "欼");
                            HbjDictionaryFx.Add("&#27708;", "氼");
                            HbjDictionaryFx.Add("&#27964;", "洼");
                            HbjDictionaryFx.Add("&#28220;", "渼");
                            HbjDictionaryFx.Add("&#28476;", "漼");
                            HbjDictionaryFx.Add("&#28732;", "瀼");
                            HbjDictionaryFx.Add("&#28988;", "焼");
                            HbjDictionaryFx.Add("&#29244;", "爼");
                            HbjDictionaryFx.Add("&#29500;", "猼");
                            HbjDictionaryFx.Add("&#29756;", "琼");
                            HbjDictionaryFx.Add("&#30012;", "甼");
                            HbjDictionaryFx.Add("&#30268;", "瘼");
                            HbjDictionaryFx.Add("&#30524;", "眼");
                            HbjDictionaryFx.Add("&#30780;", "砼");
                            HbjDictionaryFx.Add("&#31036;", "礼");
                            HbjDictionaryFx.Add("&#31292;", "稼");
                            HbjDictionaryFx.Add("&#31548;", "笼");
                            HbjDictionaryFx.Add("&#31804;", "簼");
                            HbjDictionaryFx.Add("&#32060;", "紼");
                            HbjDictionaryFx.Add("&#32316;", "縼");
                            HbjDictionaryFx.Add("&#32572;", "缼");
                            HbjDictionaryFx.Add("&#32828;", "耼");
                            HbjDictionaryFx.Add("&#33084;", "脼");
                            HbjDictionaryFx.Add("&#33340;", "舼");
                            HbjDictionaryFx.Add("&#33596;", "茼");
                            HbjDictionaryFx.Add("&#33852;", "萼");
                            HbjDictionaryFx.Add("&#34108;", "蔼");
                            HbjDictionaryFx.Add("&#36156;", "贼");
                            HbjDictionaryFx.Add("&#39740;", "鬼");

                            // Also add russion
                            HbjDictionaryFx.Add("&#1084;", "м");
                        }
                    }

                }

                //start to replace "encoded" Chinese characters.
                lock (HbjDictionaryFx)
                {
                    foreach (string key in HbjDictionaryFx.Keys)
                    {
                        if (returnString.Contains(key))
                        {
                            returnString = returnString.Replace(key, HbjDictionaryFx[key]);
                        }
                    }
                }
            }

            return returnString;
        }
    }
}
