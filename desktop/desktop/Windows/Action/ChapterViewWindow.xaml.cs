using library.Core.Models.Responses;
using Microsoft.Web.WebView2.Core;
using System.Windows;
using System.Windows.Input;

namespace desktop.Windows.Action
{
    public partial class ChapterViewWindow : Window
    {
        private readonly ChapterDetailResponse _chapter;

        public ChapterViewWindow(ChapterDetailResponse chapter)
        {
            InitializeComponent();

            Owner = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);
            _chapter = chapter;

            Loaded += (s, e) => LoadChapter();
        }

        private async void LoadChapter()
        {
            ChapterTitleText.Text = _chapter.Title;
            ChapterMetaText.Text = $"Глава {_chapter.ChapterNumber} • {_chapter.WordCount:N0} слов • {_chapter.Views:N0} просмотров";

            var options = new CoreWebView2EnvironmentOptions();
            var env = await CoreWebView2Environment.CreateAsync(null, null, options);
            await ContentWebView.EnsureCoreWebView2Async(env);

            var html = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <style>
                        body {{
                            font-family: 'Georgia', serif;
                            font-size: 16px;
                            line-height: 1.8;
                            color: #e0e0e0;
                            background: rgba(26, 20, 47, 0.9);
                            padding: 20px;
                            margin: 0;
                            overflow-y: auto;
                            scrollbar-width: none;
                            -ms-overflow-style: none;
                        }}
                        body::-webkit-scrollbar {{
                            width: 0;
                            height: 0;
                            display: none;
                        }}
                        
                        h1, h2, h3, h4, h5, h6 {{
                            color: #e0b0ff;
                            margin: 1.5em 0 0.5em;
                        }}
                        
                        h1:first-child, h2:first-child, h3:first-child {{
                            margin-top: 0;
                        }}
                        
                        p {{
                            margin: 1em 0;
                            text-indent: 30px;
                        }}
                        
                        p:first-letter {{
                            font-size: 1.3em;
                            font-weight: bold;
                            color: #8a2be2;
                        }}
                        
                        blockquote {{
                            border-left: 3px solid #8a2be2;
                            margin: 1em 0;
                            padding-left: 1rem;
                            font-style: italic;
                            color: rgba(224, 176, 255, 0.8);
                        }}
                        
                        hr {{
                            border: none;
                            height: 1px;
                            background: linear-gradient(90deg, transparent, #8a2be2, transparent);
                            margin: 2rem 0;
                        }}
                        
                        ul, ol {{
                            margin: 1em 0;
                            padding-left: 1.5rem;
                        }}
                        
                        a {{
                            color: #8a2be2;
                            text-decoration: none;
                        }}
                        
                        a:hover {{
                            color: #FF3C82;
                        }}
                        
                        img {{
                            max-width: 100%;
                            border-radius: 12px;
                            margin: 1rem 0;
                        }}
                        
                        pre {{
                            background: rgba(15, 12, 26, 0.8);
                            padding: 1rem;
                            border-radius: 12px;
                            overflow-x: auto;
                            font-family: monospace;
                        }}
                        
                        code {{
                            background: rgba(138, 43, 226, 0.15);
                            padding: 0.2rem 0.4rem;
                            border-radius: 6px;
                        }}
                        
                        /* Стили для таблиц */
                        table {{
                            border-collapse: collapse;
                            margin: 1rem 0;
                            width: 100%;
                        }}
                        
                        th, td {{
                            border: 1px solid rgba(138, 43, 226, 0.3);
                            padding: 8px 12px;
                            vertical-align: top;
                            text-align: left;
                        }}
                        
                        th {{
                            background: rgba(138, 43, 226, 0.15);
                            font-weight: bold;
                        }}
                        
                        /* Стили для чек-листов */
                        ul[data-type='taskList'] {{
                            list-style: none;
                            padding-left: 0;
                        }}
                        
                        ul[data-type='taskList'] li {{
                            display: flex;
                            gap: 8px;
                            align-items: center;
                        }}
                        
                        ul[data-type='taskList'] li input {{
                            margin: 0;
                            width: 18px;
                            height: 18px;
                            cursor: pointer;
                        }}
                        
                        mark {{
                            background-color: rgba(138, 43, 226, 0.3);
                            color: inherit;
                            padding: 0 2px;
                            border-radius: 4px;
                        }}
                    </style>
                    <script>
                        window.addEventListener('load', function() {{
                            var style = document.createElement('style');
                            style.innerHTML = '::-webkit-scrollbar {{ display: none; }}';
                            document.head.appendChild(style);
                        }});
                    </script>
                </head>
                <body>
                    {_chapter.Content}
                </body>
                </html>";

            ContentWebView.CoreWebView2.NavigateToString(html);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
           => Close();

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
    }
}