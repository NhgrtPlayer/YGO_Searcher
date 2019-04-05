using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System.Threading;
using System.ComponentModel;
using Newtonsoft.Json.Linq;

namespace YGO_Searcher
{
    class Connection
    {
        const string BaseUrl = "https://db.ygoprodeck.com/api/v4/cardinfo.php?sort=new";

        HttpClient client;
        public string CardsRequest = "";

        public Connection()
        {
            client = new HttpClient();
        }

        public async Task RequestAllCardsAsync(IProgress<double> progressPercentage, IProgress<string> progressStatus)
        {
            progressStatus.Report("Retrieving Cards from database...");
            progressPercentage.Report(0);
            HttpResponseMessage response = await client.GetAsync(BaseUrl);
            progressPercentage.Report(100);
            progressStatus.Report("Retrieving Cards from database : OK !");

            progressStatus.Report("Reading received data...");
            progressPercentage.Report(0);
            HttpContent responseContent = response.Content;
            int len = (int) responseContent.Headers.ContentLength.Value;

            CardsRequest = "";
            char[] tmp = new char[1024];

            using (var reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
            {
                // Write the output.
                //Console.WriteLine(await reader.ReadToEndAsync());
                while (!reader.EndOfStream)
                {
                    int x = await reader.ReadBlockAsync(tmp, 0, 1024);
                    CardsRequest += new string(tmp, 0, x);

                    double percent = 0.0;
                    if (len > 0)
                    {
                        percent = (double) CardsRequest.Length * 100 / len;
                        progressPercentage.Report(percent);
                    }
                }
            }
            progressStatus.Report("Reading received data : OK !");
        }

        public List<Card> GetCardsFromAnswer(IProgress<double> progressPercentage, IProgress<string> progressStatus)
        {
            progressStatus.Report("Creating local cards...");
            List<Card> ToReturn = new List<Card>();
            try
            {
                JArray cardResponse = JArray.Parse(CardsRequest);
                IList<JToken> cardTokens = (cardResponse.Children().ToList())[0].Children().ToList();

                foreach (var cardToken in cardTokens)
                {
                    if (cardToken.Value<string>("type").Contains("Skill") || cardToken.Value<string>("type") == "Token")
                        continue;
                    Card newCard = new Card(cardToken);
                    ToReturn.Add(newCard);
                    if (cardTokens.Count > 0)
                        progressPercentage.Report(ToReturn.Count * 100 / cardTokens.Count);
                }
            }
            catch (Exception e)
            {
                return (ToReturn);
            }

            progressStatus.Report("Creating local cards : OK !");
            progressPercentage.Report(100);
            return (ToReturn);
        }
    }
}
