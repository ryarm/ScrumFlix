using ScrumFlix.Data;
using ScrumFlix.Models;
using System;
using System.Linq;
using System.Windows.Forms;

namespace ScrumFlix.Forms
{
    public partial class ConcessionsAdminForm : Form
    {
        public ConcessionsAdminForm()
        {
            InitializeComponent();
        }

        private void ConcessionsAdminForm_Load(object sender, EventArgs e)
        {
            LoadItems();
            LoadStockItems();
        }

        private void LoadItems()
        {
            using var context = new AppDbContext();

            var items = context.ConcessionItem
                .OrderBy(c => c.ItemName)
                .Select(c => new
                {
                    c.ConcessionItemId,
                    c.ItemName,
                    c.Price,
                    c.QuantityInStock,
                    c.Minimum,
                    c.is_active
                })
                .ToList();

            gridConcessions.DataSource = items;

            if (gridConcessions.Columns.Count > 0)
            {
                gridConcessions.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
        }

        private void LoadStockItems()
        {
            using var context = new AppDbContext();

            var items = context.ConcessionItem
                .Where(c => c.is_active)
                .OrderBy(c => c.ItemName)
                .ToList();

            comboConcessionItem.DataSource = null;
            comboConcessionItem.DataSource = items;
            comboConcessionItem.DisplayMember = "ItemName";
            comboConcessionItem.ValueMember = "ConcessionItemId";

            if (items.Count > 0)
            {
                txtStockQuantity.Text = items[0].QuantityInStock.ToString();
            }
            else
            {
                txtStockQuantity.Text = "";
            }
        }

        private void ClearCrudFields()
        {
            txtItemName.Text = "";
            txtPrice.Text = "";
            txtQuantity.Text = "";
            txtMinimum.Text = "";
        }

        private bool TryGetCrudValues(out string itemName, out decimal price, out int quantity, out int minimum)
        {
            itemName = txtItemName.Text.Trim();
            price = 0;
            quantity = 0;
            minimum = 0;

            if (string.IsNullOrWhiteSpace(itemName))
            {
                MessageBox.Show("Enter a concession name please");
                return false;
            }

            if (!decimal.TryParse(txtPrice.Text.Trim(), out price) || price < 0)
            {
                MessageBox.Show("Enter a valid price");
                return false;
            }

            if (!int.TryParse(txtQuantity.Text.Trim(), out quantity) || quantity < 0)
            {
                MessageBox.Show("Enter a valid quantity");
                return false;
            }

            if (!int.TryParse(txtMinimum.Text.Trim(), out minimum) || minimum < 0)
            {
                MessageBox.Show("Enter a valid minimum quantity");
                return false;
            }

            return true;
        }

        private int? GetSelectedGridItemId()
        {
            if (gridConcessions.CurrentRow == null)
                return null;

            var value = gridConcessions.CurrentRow.Cells["ConcessionItemId"].Value;

            if (value == null)
                return null;

            if (int.TryParse(value.ToString(), out int itemId))
                return itemId;

            return null;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!TryGetCrudValues(out string itemName, out decimal price, out int quantity, out int minimum))
                return;

            using var context = new AppDbContext();

            bool exists = context.ConcessionItem.Any(c => c.ItemName.ToLower() == itemName.ToLower());

            if (exists)
            {
                MessageBox.Show("An item with that name already exists!");
                return;
            }

            var item = new ConcessionItem
            {
                ItemName = itemName,
                Price = price,
                QuantityInStock = quantity,
                Minimum = minimum,
                is_active = true
            };

            context.ConcessionItem.Add(item);
            context.SaveChanges();

            ClearCrudFields();
            LoadItems();
            LoadStockItems();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            var itemId = GetSelectedGridItemId();

            if (itemId == null)
            {
                MessageBox.Show("Select an item from the grid to update");
                return;
            }

            if (!TryGetCrudValues(out string itemName, out decimal price, out int quantity, out int minimum))
                return;

            using var context = new AppDbContext();

            var item = context.ConcessionItem.FirstOrDefault(c => c.ConcessionItemId == itemId.Value);

            if (item == null)
            {
                MessageBox.Show("Item not found");
                return;
            }

            bool duplicateName = context.ConcessionItem.Any(c =>
                c.ConcessionItemId != item.ConcessionItemId &&
                c.ItemName.ToLower() == itemName.ToLower());

            if (duplicateName)
            {
                MessageBox.Show("Another item already has that name");
                return;
            }

            item.ItemName = itemName;
            item.Price = price;
            item.QuantityInStock = quantity;
            item.Minimum = minimum;

            context.SaveChanges();

            LoadItems();
            LoadStockItems();
        }

        private void btnDeactivate_Click(object sender, EventArgs e)
        {
            var itemId = GetSelectedGridItemId();

            if (itemId == null)
            {
                MessageBox.Show("Select an item from the grid to deactivate it");
                return;
            }

            using var context = new AppDbContext();

            var item = context.ConcessionItem.FirstOrDefault(c => c.ConcessionItemId == itemId.Value);

            if (item == null)
            {
                MessageBox.Show("Item not found");
                return;
            }

            item.is_active = false;
            context.SaveChanges();

            LoadItems();
            LoadStockItems();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadItems();
            LoadStockItems();
            ClearCrudFields();
        }

        private void btnIncreaseStock_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtStockQuantity.Text, out int qty))
            {
                qty++;
                txtStockQuantity.Text = qty.ToString();
            }
            else
            {
                txtStockQuantity.Text = "0";
            }
        }

        private void btnDecreaseStock_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtStockQuantity.Text, out int qty))
            {
                if (qty > 0)
                    qty--;

                txtStockQuantity.Text = qty.ToString();
            }
            else
            {
                txtStockQuantity.Text = "0";
            }
        }

        private void btnSaveStock_Click(object sender, EventArgs e)
        {
            if (comboConcessionItem.SelectedValue == null)
            {
                MessageBox.Show("Select an item");
                return;
            }

            if (!int.TryParse(comboConcessionItem.SelectedValue.ToString(), out int itemId))
            {
                MessageBox.Show("Invalid item selected");
                return;
            }

            if (!int.TryParse(txtStockQuantity.Text.Trim(), out int newQuantity) || newQuantity < 0)
            {
                MessageBox.Show("Enter a valid stock quantity");
                return;
            }

            using var context = new AppDbContext();

            var item = context.ConcessionItem.FirstOrDefault(c => c.ConcessionItemId == itemId);

            if (item == null)
            {
                MessageBox.Show("Item not found");
                return;
            }

            item.QuantityInStock = newQuantity;
            context.SaveChanges();

            LoadItems();
            LoadStockItems();
        }

        private void comboConcessionItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboConcessionItem.SelectedValue == null)
                return;

            if (!int.TryParse(comboConcessionItem.SelectedValue.ToString(), out int itemId))
                return;

            using var context = new AppDbContext();

            var item = context.ConcessionItem.FirstOrDefault(c => c.ConcessionItemId == itemId);

            if (item != null)
            {
                txtStockQuantity.Text = item.QuantityInStock.ToString();
            }
        }

        private void gridConcessions_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (gridConcessions.CurrentRow == null)
                return;

            txtItemName.Text = gridConcessions.CurrentRow.Cells["ItemName"].Value?.ToString();
            txtPrice.Text = gridConcessions.CurrentRow.Cells["Price"].Value?.ToString();
            txtQuantity.Text = gridConcessions.CurrentRow.Cells["QuantityInStock"].Value?.ToString();
            txtMinimum.Text = gridConcessions.CurrentRow.Cells["Minimum"].Value?.ToString();
        }

        private void btnReactivate_Click(object sender, EventArgs e)
        {
            var itemId = GetSelectedGridItemId();

            if (itemId == null)
            {
                MessageBox.Show("Select an item to reactivate it");
                return;
            }

            using var context = new AppDbContext();

            var item = context.ConcessionItem.FirstOrDefault(c => c.ConcessionItemId == itemId.Value);

            if (item == null)
            {
                MessageBox.Show("Item not found");
                return;
            }

            item.is_active = true;
            context.SaveChanges();

            LoadItems();
            LoadStockItems();
        }
    }
}