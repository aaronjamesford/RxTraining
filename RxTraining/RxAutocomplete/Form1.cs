using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RxAutocomplete
{
    using System.Net;
    using System.Reactive.Concurrency;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Threading.Tasks;
    using System.Windows.Threading;
    using System.Xml;
    using System.Xml.Linq;

    using System.Xml.XPath;

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            this.resultsBox.Items.Clear();

            // TODO #11 Complete the code here to implement an autocomplete feature.
            //      - Only search if the text is longer than 2 characters
            //      - Make sure you do not search if the text has not changed (pressing arrow keys etc)
            //      - Throttle the keyup events so that you only search a maximum of once every 500ms
            //      - Only subscribe to the SearchWikipedia observable until another keyup event occurs
            //      - Convert the result of SearchWikipedia into a list of strings
            //      - Be sure to observe on the dispatcher thread (Dispatcher.CurrentDispatcher)
            var keyup = Observable.FromEventPattern<KeyEventArgs>(this.searchBox, "KeyUp");
        }

        private static IObservable<string> SearchWikipedia(string query)
        {
            const string url = "http://en.wikipedia.org/w/api.php?action=opensearch&format=xml&search=";
            var webClient = new WebClient();
            var nsManager = new XmlNamespaceManager(new NameTable());
            nsManager.AddNamespace("os", "http://opensearch.org/searchsuggest2");
            var cancellable = new CancellationDisposable();
            var asyncCancel = Disposable.Create(webClient.CancelAsync);

            // Observable.Create is similar to using a Subject
            // Here it's used so the asynchronous request can be cancelled on unsubscribe.
            return Observable.Create<string>(observer =>
                {
                    webClient.DownloadStringCompleted += (sender, args) =>
                        {
                            if(args.Error != null || args.Cancelled)
                            {
                                observer.OnCompleted();
                                return;
                            }

                            var xmlDoc = XDocument.Parse(args.Result);
                            var items = xmlDoc.XPathSelectElements("/os:SearchSuggestion/os:Section/os:Item/os:Text", nsManager);

                            foreach(var item in items.TakeWhile(item => !cancellable.Token.IsCancellationRequested))
                            {
                                observer.OnNext(item.Value);
                            }

                            observer.OnCompleted();
                        };

                    webClient.DownloadStringAsync(new Uri(url + query));

                    return new CompositeDisposable(cancellable, asyncCancel, webClient);
                });
        }
    }
}
