using System.Collections.Generic;

namespace ActivityCenter.Models{
    public class HomeView{
        public User loggedUser;
        public List<Happening> AllHappenings{get;set;}
    }
}