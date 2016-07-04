using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Net.Http;
using System.Collections.Generic;
using Square.Picasso;
using System.Net.WebSockets;


namespace ChatApplication.Android
{
    [Activity(Label = "ChatApplication.Android", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        int count = 1;

        private EditText messageInput;
        private static string END_POINT = "http://localhost:55396/";
        private MessageAdapter messageAdapter;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it

            //get our input field by its ID
            messageInput = (EditText)FindViewById<EditText>(Resource.Id.message_input);

            // get our button by its ID
            var sendButton = (Button)FindViewById(Resource.Id.send_button);

            // set its click listener
            sendButton.Click += delegate { this.PostMessage(); };

            //button.Click += delegate { button.Text = string.Format("{0} clicks!", count++); };

             messageAdapter = new MessageAdapter(this, new List<Message>());
             ListView messagesView = (ListView)FindViewById(Resource.Id.messages_view);
        }

        private void PostMessage()
        {
            String text = messageInput.Text.ToString();

            Toast.MakeText(this.ApplicationContext, "Test send message", ToastLength.Long).Show();

            // return if the text is blank
            if (text.Equals("")) {
                return;
            }

            var postData = new List<KeyValuePair<string,string>>();
            postData.Add(new KeyValuePair<string,string>("Name", text));
            HttpContent content = new FormUrlEncodedContent(postData); 
            HttpClient client = new HttpClient();

            client.PostAsync(END_POINT, content).ContinueWith((postTask) => { postTask.Result.EnsureSuccessStatusCode(); messageInput.SetText("", TextView.BufferType.Normal); });
        }
    }

    public class Message : Java.Lang.Object
    {
        public String Text { get; set; }
        public String Name { get; set; }
    }

    public class MessageAdapter : BaseAdapter
    {
        Context messageContext;
        List<Message> messageList;

        public MessageAdapter(Context c, List<Message> msg)
        {
            messageList = msg;
            messageContext = c;
        }


        public void Add(Message message)
        {
            messageList.Add(message);
            NotifyDataSetChanged();
        }

        public override int Count
        {
            get { return messageList.Count; }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return messageList[position];
        }

        public override long GetItemId(int position)
        {
            return 0;
        }


        class MessageViewHolder : Java.Lang.Object
        {
            public ImageView thumbnailImageView;
            public TextView senderView;
            public TextView bodyView;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            MessageViewHolder holder;

            if (convertView == null)
            {
                LayoutInflater messageInflater = (LayoutInflater)messageContext.GetSystemService(Activity.LayoutInflaterService);

                convertView = messageInflater.Inflate(Resource.Layout.MessageLayout, null);

                holder = new MessageViewHolder();

                holder.thumbnailImageView = convertView.FindViewById<ImageView>(Resource.Id.img_thumbnail);
                holder.senderView = convertView.FindViewById<TextView>(Resource.Id.message_sender);
                holder.bodyView = convertView.FindViewById<TextView>(Resource.Id.message_body);

                convertView.SetTag(1, holder);
            }
            else
            {
                holder = (MessageViewHolder)convertView.Tag;
            }

            Message message = (Message)GetItem(position);
            holder.bodyView.Text = (message.Text);
            holder.senderView.Text = (message.Name);

            Picasso.With(messageContext)
                .Load("https://twitter.com/" + message.Name + "/profile_image?size=original")
                .Placeholder(Resource.Drawable.def6)
                .Into(holder.thumbnailImageView);

            return convertView;
        }
    }
}

