namespace LibraryApp.ViewModels
{
    public class ItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate BookTemplate { get; set; }
        public DataTemplate UserTemplate { get; set; }
        public DataTemplate InventoryTemplate { get; set; }

        // Override the abstract method to return the appropriate template based on the item type
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            // Check the type of item and return the corresponding template
            if (item is Book)
            {
                return BookTemplate;  // Return the Book template if item is a Book
            }
            else if (item is User)
            {
                return UserTemplate;  // Return the User template if item is a User
            }
            else if (item is Inventory)
            {
                return InventoryTemplate;  // Return the Inventory template if item is an Inventory
            }

            return null;  // If no match, return null or a default template if necessary
        }
    }
}
