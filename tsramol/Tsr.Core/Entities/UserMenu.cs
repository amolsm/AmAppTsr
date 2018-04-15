using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsr.Core.Entities
{
    public class MainMenu
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
    public class SubMenu
    {
        public int Id { get; set; }
        public int MainMenuId { get; set; }
        public string Name { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public bool IsActive { get; set; }
    }
    public class UserMenu
    {
        public int Id { get; set; }
        public string RoleId { get; set; }
        public int MainMenuId { get; set; }
        public int SubMenuId { get; set; }
    }
}
