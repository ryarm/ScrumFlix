namespace ScrumFlix.Forms
{
    partial class EmployeeConcessionPOSForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            comboItems = new ComboBox();
            txtQuantity = new TextBox();
            btnAddToCart = new Button();
            gridCart = new DataGridView();
            lblTotal = new Label();
            btnRemoveItem = new Button();
            btnClearCart = new Button();
            btnCheckout = new Button();
            txtEmail = new TextBox();
            ((System.ComponentModel.ISupportInitialize)gridCart).BeginInit();
            SuspendLayout();
            // 
            // comboItems
            // 
            comboItems.FormattingEnabled = true;
            comboItems.Location = new Point(63, 40);
            comboItems.Name = "comboItems";
            comboItems.Size = new Size(158, 29);
            comboItems.TabIndex = 0;
            // 
            // txtQuantity
            // 
            txtQuantity.Location = new Point(278, 40);
            txtQuantity.Name = "txtQuantity";
            txtQuantity.PlaceholderText = "Quantity";
            txtQuantity.Size = new Size(130, 29);
            txtQuantity.TabIndex = 1;
            // 
            // btnAddToCart
            // 
            btnAddToCart.Location = new Point(488, 38);
            btnAddToCart.Name = "btnAddToCart";
            btnAddToCart.Size = new Size(98, 30);
            btnAddToCart.TabIndex = 2;
            btnAddToCart.Text = "Add to cart";
            btnAddToCart.UseVisualStyleBackColor = true;
            btnAddToCart.Click += btnAddToCart_Click;
            // 
            // gridCart
            // 
            gridCart.AllowUserToAddRows = false;
            gridCart.AllowUserToDeleteRows = false;
            gridCart.AllowUserToResizeColumns = false;
            gridCart.AllowUserToResizeRows = false;
            gridCart.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridCart.Location = new Point(59, 128);
            gridCart.MultiSelect = false;
            gridCart.Name = "gridCart";
            gridCart.ReadOnly = true;
            gridCart.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            gridCart.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridCart.Size = new Size(626, 195);
            gridCart.TabIndex = 3;
            // 
            // lblTotal
            // 
            lblTotal.AutoSize = true;
            lblTotal.Location = new Point(89, 412);
            lblTotal.Name = "lblTotal";
            lblTotal.Size = new Size(40, 21);
            lblTotal.TabIndex = 4;
            lblTotal.Text = "0.00";
            // 
            // btnRemoveItem
            // 
            btnRemoveItem.Location = new Point(141, 407);
            btnRemoveItem.Name = "btnRemoveItem";
            btnRemoveItem.Size = new Size(183, 30);
            btnRemoveItem.TabIndex = 5;
            btnRemoveItem.Text = "Remove Selected Item";
            btnRemoveItem.UseVisualStyleBackColor = true;
            btnRemoveItem.Click += btnRemoveItem_Click;
            // 
            // btnClearCart
            // 
            btnClearCart.Location = new Point(330, 407);
            btnClearCart.Name = "btnClearCart";
            btnClearCart.Size = new Size(98, 30);
            btnClearCart.TabIndex = 6;
            btnClearCart.Text = "Clear Cart";
            btnClearCart.UseVisualStyleBackColor = true;
            btnClearCart.Click += btnClearCart_Click;
            // 
            // btnCheckout
            // 
            btnCheckout.Location = new Point(462, 410);
            btnCheckout.Name = "btnCheckout";
            btnCheckout.Size = new Size(98, 30);
            btnCheckout.TabIndex = 7;
            btnCheckout.Text = "Checkout";
            btnCheckout.UseVisualStyleBackColor = true;
            btnCheckout.Click += btnCheckout_Click;
            // 
            // txtEmail
            // 
            txtEmail.Location = new Point(380, 347);
            txtEmail.Name = "txtEmail";
            txtEmail.PlaceholderText = "Customers Email";
            txtEmail.Size = new Size(130, 29);
            txtEmail.TabIndex = 8;
            // 
            // EmployeeConcessionPOSForm
            // 
            AutoScaleDimensions = new SizeF(9F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(797, 556);
            Controls.Add(txtEmail);
            Controls.Add(btnCheckout);
            Controls.Add(btnClearCart);
            Controls.Add(btnRemoveItem);
            Controls.Add(lblTotal);
            Controls.Add(gridCart);
            Controls.Add(btnAddToCart);
            Controls.Add(txtQuantity);
            Controls.Add(comboItems);
            Name = "EmployeeConcessionPOSForm";
            Text = "EmployeeConcessionPOSForm";
            FormClosed += EmployeeConcessionPOSForm_FormClosed;
            Load += EmployeeConcessionPOSForm_Load;
            ((System.ComponentModel.ISupportInitialize)gridCart).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox comboItems;
        private TextBox txtQuantity;
        private Button btnAddToCart;
        private DataGridView gridCart;
        private Label lblTotal;
        private Button btnRemoveItem;
        private Button btnClearCart;
        private Button btnCheckout;
        private TextBox txtEmail;
    }
}