using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notebook
{

    interface IMenuItem : IEnumerable<IMenuItem>
    {
        string Text { get; }
        IReadOnlyList<IMenuItem> Children { get; }

        void InvokeHandler();
    }

    class Menu
    {
        readonly IMenuItem _root;

        public Menu(IMenuItem root)
        {
            _root = root;
        }

        public void RunMenu()
        {
            this.RunMenuImpl(_root);
        }

        private void RunMenuImpl(IMenuItem currMenu)
        {
            var selectedMenu = this.SelectMenu(currMenu);

            while (selectedMenu != null)
            {
                if (selectedMenu.Children.Count > 0)
                    this.RunMenuImpl(selectedMenu);
                else
                    selectedMenu.InvokeHandler();

                selectedMenu = this.SelectMenu(currMenu);
            }
        }

        private IMenuItem SelectMenu(IMenuItem currMenu)
        {
            //Console.Clear();
            Console.WriteLine(currMenu.Text + ":");

            currMenu.ForEeach((item, n) => Console.WriteLine($"{n + 1}. {item.Text}"));
            Console.WriteLine($"{currMenu.Children.Count + 1}. Exit");

            int index;
            Console.Write("Item number: ");
            while (!int.TryParse(Console.ReadLine(), out index) || index < 1 || index > currMenu.Children.Count + 1)
            {
                Console.WriteLine("Invalid number!");
                Console.Write("Item number: ");
            }

            return index > currMenu.Children.Count ? null : currMenu.Children[index - 1];
        }

    }
}
