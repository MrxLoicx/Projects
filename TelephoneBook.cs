using System;
using System.Windows.Forms;
using System.Drawing;
using System.Xml;
namespace PhoneApp
{
	public static class Declarator{
		public static Color BackColor = Color.FromArgb(35,35,35);
		public static string ImagesPath = Application.StartupPath + @"\Images\";
		public static string PathContacsDb = Application.StartupPath + @"\contacts.xml";
	}
	
	[Serializable]
	public class Person
	{
		public string Name { get; set; }
		public string PhoneNumber { get; set; }
		public string AllContacts { get; set; }
		public string Email { get; set; }
		public string Address { get; set; }
	}
	
	public static class Api
	{
		public static XmlNodeList xmlNodeList;
		public static int currentElement = 0;
		public static int lastElement = 0;
		
		public static void AddToDb(string p_name, string p_number, string p_allconts, string p_email, string p_address)
		{
			var xdoc = new XmlDocument();
			xdoc.Load(Declarator.PathContacsDb);
			XmlElement xRoot = xdoc.DocumentElement;
		          
		    XmlElement contactElem = xdoc.CreateElement("contact");
		    XmlAttribute nameAttr = xdoc.CreateAttribute("name");
		    XmlText name = xdoc.CreateTextNode(p_name);
		    XmlElement numberElem = xdoc.CreateElement("number");
		    XmlText number = xdoc.CreateTextNode(p_number);
			XmlElement allcontactsElem = xdoc.CreateElement("all_contacts");
		    XmlText allcontacts = xdoc.CreateTextNode(p_allconts);
			XmlElement emailElem = xdoc.CreateElement("email");
		    XmlText email = xdoc.CreateTextNode(p_email);
			XmlElement addressElem = xdoc.CreateElement("address");
		    XmlText address = xdoc.CreateTextNode(p_address);
		         
		    nameAttr.AppendChild(name);
		    numberElem.AppendChild(number);
			allcontactsElem.AppendChild(allcontacts);
			emailElem.AppendChild(email);
			addressElem.AppendChild(address);
		    contactElem.Attributes.Append(nameAttr);
		    contactElem.AppendChild(numberElem);
			contactElem.AppendChild(allcontactsElem);
			contactElem.AppendChild(emailElem);
			contactElem.AppendChild(addressElem);
		    xRoot.AppendChild(contactElem);
		    xdoc.Save(Declarator.PathContacsDb);
		    xmlNodeList = xRoot.ChildNodes;
		}
		
		public static void DeleteFromDb(string p_name)
		{
			XmlDocument xdoc = new XmlDocument();
			
			xdoc.Load(Declarator.PathContacsDb);
		    var xRoot = xdoc.DocumentElement;
		        
		    XmlNode deletedNode;
		    XmlNodeList childnodes = xRoot.SelectNodes("contact");
			
			foreach(XmlNode xmlNode in childnodes)
			{
				if (xmlNode.SelectSingleNode("@name").Value == p_name) {
					deletedNode = xmlNode;
					xRoot.RemoveChild(deletedNode);
					currentElement = 0;
					break;
				}
			}
			
			xdoc.Save(Declarator.PathContacsDb);
			xmlNodeList = xRoot.ChildNodes;
		}
		
		public static Person FindFromDb(string p_name)
		{
			XmlDocument xdoc = new XmlDocument();
			xdoc.Load(Declarator.PathContacsDb);
		    var xRoot = xdoc.DocumentElement;
		    
		    XmlNodeList childnodes = xRoot.SelectNodes("contact");
			
			foreach(XmlNode xmlNode in childnodes)
			{
				if (xmlNode.SelectSingleNode("@name").Value == p_name) {
					Person person = new Person();
					person.Name = p_name;
					person.PhoneNumber = xmlNode.FirstChild.InnerText;
					person.AllContacts = xmlNode.SelectSingleNode("all_contacts").InnerText;
					person.Email = xmlNode.SelectSingleNode("email").InnerText;
					person.Address = xmlNode.SelectSingleNode("address").InnerText;
					return person;
				}
			}
		    
			return null;
		}
		
		public static Person GetNextElement()
		{
			if (currentElement == xmlNodeList.Count-1) { currentElement = 0; }
			else { ++currentElement;} 
				
			return GetCurrentPerson(currentElement);
		}
		
		public static Person GetPreviousElement()
		{
			if (currentElement == 0) { currentElement = xmlNodeList.Count-1; }
			else { --currentElement;}
			
			return GetCurrentPerson(currentElement);
		}
		
		public static Person GetCurrentPerson(int current)
		{
			Person person = new Person();
			person.Name = xmlNodeList[current].SelectSingleNode("@name").Value;
			person.PhoneNumber = xmlNodeList[current].FirstChild.InnerText;
			person.AllContacts = xmlNodeList[current].SelectSingleNode("all_contacts").InnerText;
			person.Email = xmlNodeList[current].SelectSingleNode("email").InnerText;
			person.Address = xmlNodeList[current].SelectSingleNode("address").InnerText;
			return person;
		}
		
		public static int GetCountContacts()
		{
			XmlDocument xdoc = new XmlDocument();
			xdoc.Load(Declarator.PathContacsDb);
			var xRoot = xdoc.DocumentElement;
			xmlNodeList = xRoot.ChildNodes;
			return xRoot.SelectNodes("contact").Count;
		}
	}
	
	public class PhoneForm : Form
	{
		Panel panel1 = new Panel();
		Label label1 = new Label();
		PictureBox pCollapse = new PictureBox();
		PictureBox pExpand = new PictureBox();
		PictureBox pClose = new PictureBox();
		Button bAdd = new Button();
		Button bFind = new Button();
		Button bDelete = new Button();
		Label lUser = new Label();
		Label lAllContacts = new Label();
		Label lPhone = new Label();
	    Label lContacts = new Label();
	    Label lEmail = new Label();
	    Label lAddress = new Label();
	    Button bLeft = new Button();
	    Button bRight = new Button();
	    TextBox tbContacts = new TextBox();
	    TextBox tbPhone = new TextBox();
	    TextBox tbUser = new TextBox();
	    TextBox tbEmail = new TextBox();
	    PictureBox pbIcon = new PictureBox();
	    TextBox tbAddress = new TextBox();
		
	    int OffsetX, OffsetY;
		bool isMouseDown = false;
	    
	    private void MouseDown_Click(object sender, MouseEventArgs e)
	    {
	      if (e.Button == MouseButtons.Left)
	      {
	        this.isMouseDown = true;
	        Point screen = this.PointToScreen(new Point(e.X, e.Y));
	        this.OffsetX = this.Location.X - screen.X;
	        this.OffsetY = this.Location.Y - screen.Y;
	      }
	      else
	        this.isMouseDown = false;
	      if (e.Clicks != 2)
	        return;
	      this.isMouseDown = false;
	    }
	
	    private void MouseMove_Click(object sender, MouseEventArgs e)
	    {
	      if (!this.isMouseDown)
	        return;
	      if (this.WindowState == FormWindowState.Maximized)
	        this.WindowState = FormWindowState.Normal;
	      Point screen = this.panel1.PointToScreen(new Point(e.X, e.Y));
	      screen.Offset(new Point(OffsetX, OffsetY));
	      this.Location = screen;
	    }
	
	    private void MouseUp_Click(object sender, MouseEventArgs e)
	    {
	      this.isMouseDown = false;
	    }
	    
	    public void ExpandForm_Click(object sender, EventArgs args) {
			if (WindowState == FormWindowState.Normal) {
				WindowState = FormWindowState.Maximized;
			} else {
				WindowState = FormWindowState.Normal;
			}
		}
		
		public void CollapseForm_Click(object sender, EventArgs args) {
			if (WindowState == FormWindowState.Normal || WindowState == FormWindowState.Maximized) {
				WindowState = FormWindowState.Minimized;
			}
		}

	    public void AddPerson_Click(object sender, EventArgs args)
	    {
	    	if (String.IsNullOrWhiteSpace(tbUser.Text)) {
	    		MessageBox.Show("Введите имя пользователя", "Ошибка");
	    		return;
	    	}
	    	
	    	Api.AddToDb(tbUser.Text, tbPhone.Text, tbContacts.Text, tbEmail.Text, tbAddress.Text);
	    	lAllContacts.Text = "Всего контактов: " + Api.GetCountContacts();
	    }
	    
	    public void RemovePerson_Click(object sender, EventArgs args)
	    {
	    	if (String.IsNullOrWhiteSpace(tbUser.Text)) return;
	    	
	    	if (MessageBox.Show("Удалить контакт?", "Предупреждение") == DialogResult.OK) {
	    		Api.DeleteFromDb(tbUser.Text);
	    		lAllContacts.Text = "Всего контактов: " + Api.GetCountContacts();
	    	}
	    }
	    
	    public void FindPerson_Click(object sender, EventArgs args)
	    {
	    	if (String.IsNullOrWhiteSpace(tbUser.Text)) {
	    		MessageBox.Show("Введите имя пользователя для поиска", "Ошибка");
	    		return;
	    	}
	    	
	    	Person person = Api.FindFromDb(tbUser.Text);
	    	if (person == null) return;
	    	
	    	WriteDataPerson(ref person);
	    }
	    
	    private void WriteDataPerson(ref Person person)
	    {
	    	tbUser.Text = person.Name;
	    	tbPhone.Text = person.PhoneNumber;
	    	tbContacts.Text = person.AllContacts;
	    	tbEmail.Text = person.Email;
	    	tbAddress.Text = person.Address;
	    }
	    
	    public void NextPerson_Click(object sender, EventArgs args)
	    {
	    	Person person = Api.GetNextElement();
	    	WriteDataPerson(ref person);
	    }
	    
	    public void PreviousPerson_Click(object sender, EventArgs args)
	    {
	    	Person person = Api.GetPreviousElement();
	    	WriteDataPerson(ref person);
	    }
	    
	    public void Load_Form(object sender, EventArgs args)
	    {
	    	lAllContacts.Text = "Всего контактов: " + Api.GetCountContacts();
	    }
	    
	    public PhoneForm()
	    {
	    	this.panel1.BackColor = Declarator.BackColor;
        this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.panel1.Controls.Add(this.label1);
        this.panel1.Controls.Add(this.pExpand);
        this.panel1.Controls.Add(this.pCollapse);
        this.panel1.Controls.Add(this.pClose);
        this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
        this.panel1.Location = new System.Drawing.Point(0, 0);
        this.panel1.Size = new System.Drawing.Size(621, 36);
        this.panel1.MouseDown += MouseDown_Click;
		this.panel1.MouseMove += MouseMove_Click;
		this.panel1.MouseUp += MouseUp_Click;
        
        this.pClose.BackgroundImage = Image.FromFile(Declarator.ImagesPath + "close.png");
        this.pClose.BackgroundImageLayout = ImageLayout.Zoom;
        this.pClose.Anchor = (AnchorStyles)AnchorStyles.Top | AnchorStyles.Right;
        this.pClose.Location = new System.Drawing.Point(582, 0);
        this.pClose.Size = new System.Drawing.Size(36, 31);
        this.pClose.MouseHover += (o,e) => { this.pClose.BackgroundImage = Image.FromFile(Declarator.ImagesPath + "close_hover.png"); };
        this.pClose.MouseLeave += (o,e) => { this.pClose.BackgroundImage = Image.FromFile(Declarator.ImagesPath + "close.png"); };
        this.pClose.Click += (o,e) => Application.Exit();
        
        this.pExpand.BackgroundImage = Image.FromFile(Declarator.ImagesPath + "expand.png");
        this.pExpand.BackgroundImageLayout = ImageLayout.Zoom;
        this.pExpand.Anchor = (AnchorStyles)AnchorStyles.Top | AnchorStyles.Right;
        this.pExpand.Location = new System.Drawing.Point(547, 0);
        this.pExpand.Size = new System.Drawing.Size(36, 34);
        this.pExpand.MouseHover += (o,e) => { this.pExpand.BackgroundImage = Image.FromFile(Declarator.ImagesPath + "expand_hover.png"); };
        this.pExpand.MouseLeave += (o,e) => { this.pExpand.BackgroundImage = Image.FromFile(Declarator.ImagesPath + "expand.png"); };
        this.pExpand.Click += ExpandForm_Click;
        
        this.pCollapse.BackgroundImage = Image.FromFile(Declarator.ImagesPath + "collapse.png");
        this.pCollapse.BackgroundImageLayout = ImageLayout.Zoom;
        this.pCollapse.Anchor = (AnchorStyles)AnchorStyles.Top | AnchorStyles.Right;
        this.pCollapse.Location = new System.Drawing.Point(512, 0);
        this.pCollapse.Size = new System.Drawing.Size(36, 34);
        this.pCollapse.MouseHover += (o,e) => { this.pCollapse.BackgroundImage = Image.FromFile(Declarator.ImagesPath + "collapse_hover.png"); };
        this.pCollapse.MouseLeave += (o,e) => { this.pCollapse.BackgroundImage = Image.FromFile(Declarator.ImagesPath + "collapse.png"); };
        this.pCollapse.Click += CollapseForm_Click;
        
        this.label1.Font = new System.Drawing.Font("Consolas", 14.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (System.Byte)0);
        this.label1.ForeColor = System.Drawing.Color.LightGreen;
        this.label1.Location = new System.Drawing.Point(4, 6);
        this.label1.Size = new System.Drawing.Size(312, 28);
        this.label1.Text = "Контакты v1.0.0.0";
        this.label1.MouseDown += MouseDown_Click;
		this.label1.MouseMove += MouseMove_Click;
		this.label1.MouseUp += MouseUp_Click;
        
        this.bAdd.Anchor = (System.Windows.Forms.AnchorStyles)System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
        this.bAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.bAdd.Font = new System.Drawing.Font("Consolas", 14.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (System.Byte)0);
        this.bAdd.ForeColor = System.Drawing.Color.LightGreen;
        this.bAdd.Location = new System.Drawing.Point(478, 46);
        this.bAdd.Size = new System.Drawing.Size(40, 37);
        this.bAdd.Text = "+";
        this.bAdd.Click += AddPerson_Click;
        
        this.bFind.Anchor = (System.Windows.Forms.AnchorStyles)System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
        this.bFind.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.bFind.Font = new System.Drawing.Font("Consolas", 14.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (System.Byte)0);
        this.bFind.ForeColor = System.Drawing.Color.LightGreen;
        this.bFind.Location = new System.Drawing.Point(523, 46);
        this.bFind.Size = new System.Drawing.Size(40, 37);
        this.bFind.Text = "()";
        this.bFind.Click += FindPerson_Click;
        
        this.bDelete.Anchor = (AnchorStyles)AnchorStyles.Top | AnchorStyles.Right;
        this.bDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.bDelete.Font = new System.Drawing.Font("Consolas", 14.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (System.Byte)0);
        this.bDelete.ForeColor = System.Drawing.Color.LightGreen;
        this.bDelete.Location = new System.Drawing.Point(569, 46);
        this.bDelete.Size = new System.Drawing.Size(40, 37);
        this.bDelete.Text = "x";
        this.bDelete.Click += RemovePerson_Click;
        
        this.lUser.Font = new System.Drawing.Font("Comic Sans MS", 12, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (System.Byte)204);
        this.lUser.ForeColor = System.Drawing.Color.LightGreen;
        this.lUser.Location = new System.Drawing.Point(5, 96);
        this.lUser.Size = new System.Drawing.Size(173, 28);
        this.lUser.Text = "Пользователь";
        
        this.lAllContacts.Font = new System.Drawing.Font("Consolas", 12, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (System.Byte)204);
        this.lAllContacts.ForeColor = System.Drawing.Color.LightGreen;
        this.lAllContacts.Location = new System.Drawing.Point(169, 55);
        this.lAllContacts.Size = new System.Drawing.Size(211, 28);
        this.lAllContacts.Text = "Всего контактов: 0";
        
        this.lPhone.Font = new System.Drawing.Font("Comic Sans MS", 12, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (System.Byte)204);
        this.lPhone.ForeColor = System.Drawing.Color.LightGreen;
        this.lPhone.Location = new System.Drawing.Point(5, 133);
        this.lPhone.Size = new System.Drawing.Size(173, 28);
        this.lPhone.Text = "Номер телефона:";
        
        this.lContacts.Font = new System.Drawing.Font("Comic Sans MS", 12, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (System.Byte)204);
        this.lContacts.ForeColor = System.Drawing.Color.LightGreen;
        this.lContacts.Location = new System.Drawing.Point(5, 172);
        this.lContacts.Size = new System.Drawing.Size(211, 28);
        this.lContacts.Text = "Связи в других сетях:";
        
        this.lEmail.Font = new System.Drawing.Font("Comic Sans MS", 12, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (System.Byte)204);
        this.lEmail.ForeColor = System.Drawing.Color.LightGreen;
        this.lEmail.Location = new System.Drawing.Point(5, 207);
        this.lEmail.Size = new System.Drawing.Size(181, 28);
        this.lEmail.Text = "Email:";
        
        this.lAddress.Font = new System.Drawing.Font("Comic Sans MS", 12, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (System.Byte)204);
        this.lAddress.ForeColor = System.Drawing.Color.LightGreen;
        this.lAddress.Location = new System.Drawing.Point(5, 243);
        this.lAddress.Size = new System.Drawing.Size(191, 28);
        this.lAddress.Text = "Адрес проживания:";
        
        this.bLeft.Anchor = (AnchorStyles)AnchorStyles.Top | AnchorStyles.Right;
        this.bLeft.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.bLeft.Font = new System.Drawing.Font("Consolas", 14.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (System.Byte)0);
        this.bLeft.ForeColor = System.Drawing.Color.LightGreen;
        this.bLeft.Location = new System.Drawing.Point(388, 46);
        this.bLeft.Size = new System.Drawing.Size(40, 37);
        this.bLeft.Text = "<";
        this.bLeft.Click += NextPerson_Click;
        
        this.bRight.Anchor = (AnchorStyles)AnchorStyles.Top | AnchorStyles.Right;
        this.bRight.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.bRight.Font = new System.Drawing.Font("Consolas", 14.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (System.Byte)0);
        this.bRight.ForeColor = System.Drawing.Color.LightGreen;
        this.bRight.Location = new System.Drawing.Point(433, 46);
        this.bRight.Size = new System.Drawing.Size(40, 37);
        this.bRight.Text = ">";
        this.bRight.Click += PreviousPerson_Click;
        
        this.tbContacts.BackColor = Declarator.BackColor;
        this.tbContacts.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.tbContacts.Font = new System.Drawing.Font("Comic Sans MS", 14.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (System.Byte)0);
        this.tbContacts.ForeColor = System.Drawing.Color.LightGreen;
        this.tbContacts.Location = new System.Drawing.Point(206, 167);
        this.tbContacts.Size = new System.Drawing.Size(403, 34);
        
        this.tbPhone.BackColor = Declarator.BackColor;
        this.tbPhone.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.tbPhone.Font = new System.Drawing.Font("Comic Sans MS", 14.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (System.Byte)0);
        this.tbPhone.ForeColor = System.Drawing.Color.LightGreen;
        this.tbPhone.Location = new System.Drawing.Point(206, 129);
        this.tbPhone.Size = new System.Drawing.Size(403, 34);
        
        this.tbUser.BackColor = Declarator.BackColor;
        this.tbUser.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.tbUser.Font = new System.Drawing.Font("Comic Sans MS", 14.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (System.Byte)0);
        this.tbUser.ForeColor = System.Drawing.Color.LightGreen;
        this.tbUser.Location = new System.Drawing.Point(206, 92);
        this.tbUser.Size = new System.Drawing.Size(403, 34);
        
        this.tbEmail.BackColor = Declarator.BackColor;
        this.tbEmail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.tbEmail.Font = new System.Drawing.Font("Comic Sans MS", 14.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (System.Byte)0);
        this.tbEmail.ForeColor = System.Drawing.Color.LightGreen;
        this.tbEmail.Location = new System.Drawing.Point(206, 204);
        this.tbEmail.Size = new System.Drawing.Size(403, 34);
        
        this.tbAddress.BackColor = Declarator.BackColor;
        this.tbAddress.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.tbAddress.Font = new System.Drawing.Font("Comic Sans MS", 14.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (System.Byte)0);
        this.tbAddress.ForeColor = System.Drawing.Color.LightGreen;
        this.tbAddress.Location = new System.Drawing.Point(206, 241);
        this.tbAddress.Size = new System.Drawing.Size(403, 34);
        
        this.pbIcon.BackgroundImage = Image.FromFile(Declarator.ImagesPath + "vk.jpg");
        this.pbIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
        this.pbIcon.Location = new System.Drawing.Point(9, 38);
        this.pbIcon.Size = new System.Drawing.Size(128, 55);
        
        this.BackColor = Declarator.BackColor;
        this.ClientSize = new System.Drawing.Size(621, 299);
        this.Controls.Add(this.pbIcon);
        this.Controls.Add(this.tbAddress);
        this.Controls.Add(this.tbEmail);
        this.Controls.Add(this.tbUser);
        this.Controls.Add(this.tbPhone);
        this.Controls.Add(this.tbContacts);
        this.Controls.Add(this.bRight);
        this.Controls.Add(this.bLeft);
        this.Controls.Add(this.lAddress);
        this.Controls.Add(this.lEmail);
        this.Controls.Add(this.lContacts);
        this.Controls.Add(this.lPhone);
        this.Controls.Add(this.lAllContacts);
        this.Controls.Add(this.lUser);
        this.Controls.Add(this.bDelete);
        this.Controls.Add(this.bAdd);
        this.Controls.Add(this.bFind);
        this.Controls.Add(this.panel1);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        this.Load += Load_Form;
	    }
	}
	
	class Program
	{
		public static void Main(string[] args)
		{
			PhoneForm phoneForm = new PhoneForm();
			Application.Run(phoneForm);
		}
	}
}
