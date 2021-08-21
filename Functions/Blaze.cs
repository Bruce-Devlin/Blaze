using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Blaze.Functions
{
    class Blaze
    {
        public static Windows.Home home; 
        public static int PokeCount = 0;
        public static DateTime lastPokeTime;
        public static bool Hidden = false;


        public static Task Say(string txt = "", string emote = "Nutural")
        {

            try
            {
                if (!Hidden)
                {
                    if (txt != "")
                    {
                        home.BlazeTxtBox.Text = txt;
                        home.BlazeCloseBtn.Visibility = Visibility.Visible;
                        home.BlazeTxtBox.Visibility = Visibility.Visible;
                        home.BlazeTxtBG.Visibility = Visibility.Visible;
                    }

                    if (emote == "Hide")
                    {
                        home.BlazeTxtBox.Text = "";
                        home.Blaze.Visibility = Visibility.Hidden;
                        home.BlazeTxtBG.Visibility = Visibility.Hidden;
                        home.BlazeCloseBtn.Visibility = Visibility.Hidden;

                    }
                    else
                    {
                        ImageBrush ib = new ImageBrush();

                        ib.ImageSource = new BitmapImage(new Uri(@"pack://application:,,,/Assets/Blaze/Small/" + emote + "-small.png"));

                        home.Blaze.Background = ib;
                        return Task.CompletedTask;
                    }
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.ToString());
            }
            return Task.CompletedTask;
        }

        public static async Task Poke()
        {
            Random r = new Random();
            int randomInt;
            if ((DateTime.Now - lastPokeTime).TotalSeconds > 7) PokeCount = 0;
            switch (PokeCount)
            {
                case int count when count <= 10:
                    PokeCount++;
                    randomInt = r.Next(0, 5);
                    switch (randomInt)
                    {
                        case 0:
                            Say("Hello!", "Happy");
                            break;
                        case 1:
                            Say("Yes?", "Wait");
                            break;
                        case 2:
                            Say("You are looking beautiful today btw, just saying!", "Happy");
                            break;
                        case 3:
                            Say("Stop! That tickles!!", "Happy");
                            break;
                        case 4:
                            Say("Are beans as good as they look? I have heard good things but never tried them, need to get myself some beans...", "Wait");
                            break;
                        case 5:
                            Say("zzZzzZzzZZzZZzzZZZzzZZzzZZzZZzzzZZzz", "Sleepy");
                            break;
                    }
                    break;

                case int count when count > 10 && count <= 40:
                    PokeCount++;
                    randomInt = r.Next(0, 5);
                    switch (randomInt)
                    {
                        case 0:
                            Say("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA -just kidding I'm fine!", "Wow");
                            break;
                        case 1:
                            Say("Are you asking me something? I can't seem to hear you?", "Wait");
                            break;
                        case 2:
                            Say("Just keep clicking, I'm sure nothing that bad can happen?...", "Happy");
                            break;
                        case 3:
                            Say("*Bored Blaze noises*", "Sleepy");
                            break;
                        case 4:
                            Say("Still clicking?", "Wait");
                            break;
                        case 5:
                            Say("I like trains!", "Happy");
                            break;
                    }
                    break;

                case int count when count > 40 && count <= 100:
                    PokeCount++;
                    Say("Okay, you can stop now...", "Really");
                    break;

                case int count when count > 100 && count <= 150:
                    PokeCount++;
                    Say("Ya know, if you leave a mark; my developer is going to be very mad...", "Really");
                    break;

                case int count when count > 150 && count <= 200:
                    PokeCount++;
                    Say("Okay, honestly... it's kinda annoying...", "Really");
                    break;

                case int count when count > 200 && count <= 300:
                    PokeCount++;
                    Say("Surely you have better things to do than to click me...", "Really");
                    break;

                case int count when count > 300 && count <= 400:
                    PokeCount++;
                    Say("Why hasn't the sound of your own clicks not driven you insane yet?...", "Really");
                    break;

                case int count when count > 400 && count <= 500:
                    PokeCount++;
                    Say("Like your finger must hurt?", "Really");
                    break;

                case int count when count > 500 && count <= 600:
                    PokeCount++;
                    Say("Last chance; stop it.", "Really");
                    break;

                case 601:
                    PokeCount++;
                    Say("Okay, you win! I'm leaving! Bye.", "Really");
                    await Task.Delay(3000);
                    Say("Seeya!", "Hide");
                    break;
            }
            await Task.Delay(3000);
            lastPokeTime = DateTime.Now;
        }
    }
}
