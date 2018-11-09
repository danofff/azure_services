using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Args;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using System.IO;
using System.Xml.XPath;

namespace MyFaceIdApp
{
    class Program
    {
        private static string _bot_key = "";
        private static readonly TelegramBotClient TelegramBotEndpoint = new TelegramBotClient(_bot_key);
        private static UserManager _userManager = new UserManager();
        private static FaceApiManager _faceApiManager = new FaceApiManager();
        static void Main(string[] args)
        {
            var me = TelegramBotEndpoint.GetMeAsync().Result;
            Console.Title = me.Username;

            TelegramBotEndpoint.OnMessage += BotOnMessageReceived;

            TelegramBotEndpoint.StartReceiving(Array.Empty<UpdateType>());


            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();
            TelegramBotEndpoint.StopReceiving();
        }

        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;
            var userFrom = message.From;
            var loadUser = _userManager.CheckUserRegistration(userFrom.Id.ToString());

            if (message.Type == MessageType.Photo)
            {             
                if (loadUser == null)
                {
                    await TelegramBotEndpoint.SendTextMessageAsync(
                            message.Chat.Id,
                            $"Type /start for your registration");
                    return;
                }
                
                Telegram.Bot.Types.File file = await TelegramBotEndpoint.GetFileAsync(message.Photo[message.Photo.Count() - 1].FileId);
                using (FileStream Filestream = System.IO.File.Create("img" + loadUser.ChatId + ".jpg"))
                {
                    await TelegramBotEndpoint.GetInfoAndDownloadFileAsync(file.FileId, Filestream);
                    byte[] byteData = GetImageAsByteArray(Filestream);
                    _userManager.AddPhoto(loadUser, "img" + loadUser.ChatId + ".jpg");
                    var faceData = await _faceApiManager.MakeAnalysisRequest(byteData);
                    if (faceData != null)
                    {
                        _userManager.AddGenderAge(loadUser, faceData);
                        await TelegramBotEndpoint.SendTextMessageAsync(
                            message.Chat.Id,
                            $"Your photo successfully uploaded");
                        return;
                    }
                    await TelegramBotEndpoint.SendTextMessageAsync(
                        message.Chat.Id,
                        $"FaceApi Service Temporary unavalible");

                }

                return;
            }



            if (message.Type == MessageType.Text)
            {
                switch (message.Text.Split(' ').First())
                {
                    case "/start":
                        
                        if (loadUser == null)
                        {
                            UserModel model = new UserModel()
                            {
                                ChatId = userFrom.Id.ToString(),
                                UserName = userFrom.Username
                            };

                            var registeredUser = _userManager.SignUpApplicationUser(model);

                            await TelegramBotEndpoint.SendTextMessageAsync(
                            message.Chat.Id,
                            $"Hello {registeredUser.UserName}, you are signed up!,\n" +
                            $"Now you can upload your photo in jpg format");

                            break;
                        }
                        else
                        {
                            string backMessage;
                            if (loadUser.Photo != null)
                            {
                                backMessage = $"Hello {loadUser.UserName}, welcome back!\n";
                            }
                            else
                            {
                                backMessage = $"Hello {loadUser.UserName}, welcome back!\n" +
                                    $"Don't forget to upload your photo!";
                            }
                            await TelegramBotEndpoint.SendTextMessageAsync(
                            message.Chat.Id,
                            backMessage
                            );
                            break;
                        }
                    case "/getpair":
                        if (loadUser == null)
                        {
                            await TelegramBotEndpoint.SendTextMessageAsync(
                                    message.Chat.Id,
                                    $"Type /start for your registration");
                            return;
                        }
                        if (loadUser.Photo == null)
                        {
                            await TelegramBotEndpoint.SendTextMessageAsync(
                                    message.Chat.Id,
                                    $"Try to upload your photo");
                            return;
                        }

                        var pair = _userManager.GetPair(loadUser);
                        if (pair == null)
                        {
                            await TelegramBotEndpoint.SendTextMessageAsync(
                                message.Chat.Id,
                                $"No pair for you. Try later");
                            return;
                        }
                        await TelegramBotEndpoint.SendTextMessageAsync(
                            message.Chat.Id,
                            $"User name: {pair.UserName}\n" +
                            $"Age: {pair.Age}");
                        using (FileStream stream = new FileStream(pair.Photo,FileMode.Open))
                        {
                            await TelegramBotEndpoint.SendPhotoAsync(message.Chat.Id, stream);
                        }
                        break;
                }
            }
        }

        private static byte[] GetImageAsByteArray(FileStream stream)
        {
            BinaryReader binaryReader = new BinaryReader(stream);
            return binaryReader.ReadBytes((int)stream.Length);
        }
    }
}
