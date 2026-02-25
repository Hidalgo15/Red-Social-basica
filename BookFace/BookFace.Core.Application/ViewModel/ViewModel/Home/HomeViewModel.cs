using BookFace.Core.Application.ViewModel.ViewModel.Publicacion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookFace.Core.Application.ViewModel.ViewModel.Home
{
    public class HomeViewModel
    {
        public List<PublicacionViewModel> Posts { get; set; }
        public string CurrentUserName { get; set; }

    }
}
