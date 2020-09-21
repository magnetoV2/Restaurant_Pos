using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Restaurant_Pos.ViewModels.Page
{
    public class TextSearchFilter
    {
        public TextSearchFilter(
            ICollectionView filteredView,
            TextBox textBox, ListView lst)
        {
            string filterText = "";

            filteredView.Filter = delegate (object obj)
            {
                if (String.IsNullOrEmpty(filterText))
                    return true;

                string str = obj.GetType().GetProperty("SearchField").GetValue(obj, null).ToString();
                if (String.IsNullOrEmpty(str))
                    return false;

                int index = str.IndexOf(
                    filterText,
                    0,
                    StringComparison.InvariantCultureIgnoreCase);

                return index > -1;
            };

            textBox.TextChanged += delegate
            {

                filterText = textBox.Text;
                filteredView.Refresh();
                if (lst.Items.Count > 0)
                {
                    lst.SelectedIndex = 0;
                }
            };
        }
    }

}
