/** Authors & Student Number:
    Siqian Yu 200286902
    Date Modified: 06-23-2016
    File Description: This is the backend file to edit Todo List. 
    **/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

//using statements required for EF DB access
using COMP2007_S2016_MidTerm_200286902.Models;
using System.Web.ModelBinding;

namespace COMP2007_S2016_MidTerm_200286902
{
    public partial class TodoDetails : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if ((!IsPostBack) && (Request.QueryString.Count > 0))
            {
                this.GetTodoList();
            }
        }

        protected void GetTodoList()
        {
            // populate the form with existing Todo data from the db
            int TodoID = Convert.ToInt32(Request.QueryString["TodoID"]);

            // connect to the EF DB
            using (TodoConnection db = new TodoConnection())
            {
                // populate a Todo instance with the TodoID from the URL parameter
                Todo updatedTodo = (from todo in db.Todos
                                          where todo.TodoID == TodoID
                                          select todo).FirstOrDefault();

                // map the Todo properties to the form controls
                if (updatedTodo != null)
                {
                    TodoNameTextBox.Text = updatedTodo.TodoName;
                    TodoNotesTextBox.Text = updatedTodo.TodoNotes;
                    CheckBox1.Checked = updatedTodo.Completed.Value;        

       
                }
            }
        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            //Redirect to Todo List Page
            Response.Redirect("~/TodoList.aspx");
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {

            //Use EF to connect to  the server
            using (TodoConnection db = new TodoConnection())
            {
                //use the Todo model to create a new Todo object and
                //save a new record
                Todo newTodo = new Todo();

                int TodoID = 0;

                if (Request.QueryString.Count > 0)
                {
                    // get the id from url
                    TodoID = Convert.ToInt32(Request.QueryString["TodoID"]);

                    // get the current Todo from EF DB
                    newTodo = (from todo in db.Todos
                               where todo.TodoID == TodoID
                                  select todo).FirstOrDefault();
                }

                //add form data to the new Todo record
                newTodo.TodoName = TodoNameTextBox.Text;
                newTodo.TodoNotes = TodoNotesTextBox.Text;
                newTodo.Completed = CheckBoxChecked;

                //use LINQ to ADO.NET to add / insert new Todo into the database
                if (TodoID == 0)
                {
                    db.Todos.Add(newTodo);
                }


                //save changes
                db.SaveChanges();

                //Redirect back to the updated Todos page
                Response.Redirect("~/TodoList.aspx");
            }
        }

        Boolean CheckBoxChecked;

        protected void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckBox1.Checked)
                CheckBoxChecked = true;
            else
                CheckBoxChecked = false;
        }
    }
    }