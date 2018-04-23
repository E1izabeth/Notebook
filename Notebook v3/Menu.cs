using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notebook
{
    internal interface IMenuItem : IEnumerable<IMenuItem>
    {
        string Text { get; }
        IReadOnlyList<IMenuItem> Children { get; }

        void InvokeHandler();
    }

    internal class Menu
    {
        readonly IMenuItem _root;

        public Menu(IMenuItem root)
        {
            _root = root;
        }

        public void RunMenu()
        {
            RunMenuImpl(_root);
        }

        private void RunMenuImpl(IMenuItem currMenu)
        {
            var selectedMenu = SelectMenu(currMenu);

            while (selectedMenu != null)
            {
                if (selectedMenu.Children.Count > 0)
                    RunMenuImpl(selectedMenu);
                else
                    selectedMenu.InvokeHandler();

                selectedMenu = SelectMenu(currMenu);
            }
        }

        private IMenuItem SelectMenu(IMenuItem currMenu)
        {
            //Console.Clear();
            Console.WriteLine(currMenu.Text + ":");

            currMenu.ForEeach((item, n) => Console.WriteLine($"{n + 1}. {item.Text}"));
            Console.WriteLine($"{currMenu.Children.Count + 1}. " + Notebook_v3.Properties.text.Exit);

            int index;
            Console.Write(Notebook_v3.Properties.text.Number_item + ": ");
            while (!int.TryParse(Console.ReadLine(), out index) || index < 1 || index > currMenu.Children.Count + 1)
            {
                Console.WriteLine(Notebook_v3.Properties.text.Invalid_number);
                Console.Write(Notebook_v3.Properties.text.Number_item + ": ");
            }

            return index > currMenu.Children.Count ? null : currMenu.Children[index - 1];
        }

    }
}
