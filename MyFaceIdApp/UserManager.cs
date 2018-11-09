using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFaceIdApp
{
    public class UserManager
    {
        private readonly GenericRepository _userStorage;
        private static Random _rnd;

        public UserModel CheckUserRegistration(string userId)
        {
            var user = _userStorage.Users.SingleOrDefault(s => s.ChatId == userId);    
            return user;
        }
        public UserManager()
        {
            _userStorage = new GenericRepository();
            _rnd = new Random();
        }

        public UserModel SignUpApplicationUser(UserModel model)
        {
            var user = new UserModel()
            {
                ChatId = model.ChatId,
                UserName = model.UserName,
            };

            _userStorage.Users.Add(user);
            _userStorage.SaveChangesAsync();

            return user;
        }

        public void AddPhoto(UserModel user, string photoPath)
        {
            if (user != null)
            {
                user.Photo = photoPath;
                _userStorage.SaveChanges();
            }
        }

        public void AddGenderAge(UserModel user, RootObject data)
        {
            if (user != null)
            {
                user.Gender = data.faceAttributes.gender == "male" ? true : false;
                user.Age = (int)data.faceAttributes.age;
                _userStorage.SaveChanges();
            }
        }

        public UserModel GetPair(UserModel user)
        {
            var pairs = _userStorage.Users.ToList().Select(s => s).
                Where(w=>w.Age!=null&&w.Gender!=null&&w.Age >= (user.Age - 5) && w.Age <= (user.Age + 5) && w.Gender != user.Gender).
                ToList();
            if (pairs.Count == 0)
            {
                return null;
            }
            var randomPair = pairs[_rnd.Next(0, pairs.Count)];
            return randomPair;
        }
    }
}
