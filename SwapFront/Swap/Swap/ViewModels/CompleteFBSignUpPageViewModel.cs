using Swap.Enums;
using Swap.Models;
using Swap.Services;
using Swap.Views;
using System;
using System.Diagnostics;
using System.Windows.Input;
using Xamarin.Forms;

namespace Swap.ViewModels
{
    public class CompleteFBSignUpPageViewModel : BaseViewModel
    {
        private readonly string m_FacebookId;

        private string m_Massage;
        public string Message
        {
            get { return m_Massage; }
            set { SetValue(ref m_Massage, value); }
        }

        private string m_PhoneNumber;
        public string PhoneNumber
        {
            get { return m_PhoneNumber; }
            set { SetValue(ref m_PhoneNumber, value); }
        }

        private string m_City;
        public string City
        {
            get { return m_City; }
            set { SetValue(ref m_City, value); }
        }

        private string m_Neighborhood;
        public string Neighborhood
        {
            get { return m_Neighborhood; }
            set { SetValue(ref m_Neighborhood, value); }
        }

        public CompleteFBSignUpPageViewModel(string i_FacebookId)
        {
            m_FacebookId = i_FacebookId;
        }

        private ICommand m_SignUp;
        public ICommand SignUp => m_SignUp ?? (m_SignUp = new Command(async () =>
        {
            if (checkInputValidity() == true)
            {
                try
                {
                    SignUpUserResult user = await ServerFacade.Users.SignUpAsync(new SignUpUser
                    {
                        Email = app.Email,
                        Password = m_FacebookId,
                        FirstName = app.UserName,
                        CellPhone = PhoneNumber,
                        City = City,
                        LastName = Neighborhood,
                    });
                    app.UpdateUserDetails(user.Id, user.Token, app.Email, app.UserName, City, Neighborhood, PhoneNumber);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }

                await Shell.Current.GoToAsync("//mainPage/home");
            }
        }));
       
        private bool checkInputValidity()
        {
            if (string.IsNullOrWhiteSpace(PhoneNumber) == true)
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(PhoneNumber))
            {
                if (StringValidationService.IsValid(PhoneNumber, ValidationType.PhoneNumber) == false)
                {
                    Message = "מספר טלפון לא תקין";
                    return false;
                }
            }

            return true;
        }
    }
}
