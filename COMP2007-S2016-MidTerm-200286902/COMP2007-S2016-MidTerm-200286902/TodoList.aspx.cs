﻿/** Authors & Student Number:
    Siqian Yu 200286902
    Date Modified: 06-23-2016
    File Description: This is the backend file to display Todo List. 
    **/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

//using statements required to connect to the EF database
using COMP2007_S2016_MidTerm_200286902.Models;
using System.Web.ModelBinding;
using System.Linq.Dynamic;


namespace COMP2007_S2016_MidTerm_200286902
{
    public partial class TodoList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //if loading the page for the first time, populate the Todos grid
            if (!IsPostBack)
            {
                Session["SortColumn"] = "TodoName"; //default sort column
                Session["SortDirection"] = "ASC";

                //get the Todo data
                this.GetTodos();
            }
        }

        /**
         * <summary>
         * This method gets the Todo data from the DB
         * </summary>
         *
         * @method GetTodos
         * @returns {void}
         */
        protected void GetTodos()
        {
            //connect to EF
            using (TodoConnection db = new TodoConnection())
            {
                string SortString = Session["SortColumn"].ToString() + " " + Session["SortDirection"].ToString();

                //query the Todos Table using EF and LINQ
                var Todos = (from allTodos in db.Todos
                                select allTodos);

                //bind the result to the GridView
                TodoGridView.DataSource = Todos.AsQueryable().OrderBy(SortString).ToList();
                //StudentsGridView.DataSource = Students.ToList();
                TodoGridView.DataBind();
            }
        }

        /**
         * <summary>
         * This event handler deletes a Todo from the db using EF
         * </summary>
         *
         *@method TodoGridView_RowDeleting
         *@param {object} sender
         *@param {GridViewDeleteEventArgs} e
         *@returns {void}
         */
        protected void TodoGridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            //store which row was clicked
            int selectedRow = e.RowIndex;

            //get the selected TodoID using the Grid's Datakey collection
            int TodoID = Convert.ToInt32(TodoGridView.DataKeys[selectedRow].Values["TodoID"]);

            //use EF to find the selected Todo in the DB and remove it
            using (TodoConnection db = new TodoConnection())
            {
                //create object of the Todo class and store the query string inside of it
                Todo deletedTodo = (from todoRecords in db.Todos
                                          where todoRecords.TodoID == TodoID
                                          select todoRecords).FirstOrDefault();

                //remove the selected student from the db
                db.Todos.Remove(deletedTodo);

                //save my changes back to the database
                db.SaveChanges();

                //refresh the grid
                this.GetTodos();
            }
        }

        /**
         *
         * <summary>
         * This event handler allows pagination to occur for the Todos Page
         *</summary>
         *
         *@method TodoGridView_PageIndexChanging
         *@param {object} sender
         *@param {GridViewPageEventArgs} e
         *@returns {void}
         */
        protected void TodoGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            //set the new page number
            TodoGridView.PageIndex = e.NewPageIndex;

            //refresh the grid
            this.GetTodos();

        }

        protected void PageSizeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Set the new Page Size
            TodoGridView.PageSize = Convert.ToInt32(PageSizeDropDownList.SelectedValue);

            //refresh the grid
            this.GetTodos();

        }
        /**
         * <summary>
         * This is the sort function
         * @returns {void}
         */
        protected void TodoGridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            //get the column to sort by
            Session["SortColumn"] = e.SortExpression;


            //Refresh the grid
            this.GetTodos();

            //toggle the direction
            Session["SortDirection"] = Session["SortDirection"].ToString() == "ASC" ? "DESC" : "ASC";

        }

        protected void TodoGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (IsPostBack)
            {
                if (e.Row.RowType == DataControlRowType.Header)//if header row has been clicked
                {
                    LinkButton linkbutton = new LinkButton();

                    for (int index = 0; index < TodoGridView.Columns.Count - 1; index++)
                    {
                        if (TodoGridView.Columns[index].SortExpression == Session["SortColumn"].ToString())
                        {
                            if (Session["SortDirection"].ToString() == "ASC")
                            {
                                linkbutton.Text = " <i class='fa fa-caret-up fa-lg'></i>";
                            }
                            else
                            {
                                linkbutton.Text = " <i class='fa fa-caret-down fa-lg'></i>";
                            }

                            e.Row.Cells[index].Controls.Add(linkbutton);
                        }
                    }
                }
            }
        }


    }
}