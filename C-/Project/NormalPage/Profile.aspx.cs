﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Data.SqlClient;
using ChocuModel;
using System.IO;

namespace Project.NormalPage
{
    public partial class Profile : System.Web.UI.Page
    {
        private int rateScore;
        private int rateNumber;

        private string connStr = WebConfigurationManager.ConnectionStrings["MyConn"].ConnectionString;

        private const int pageSize = 30;

        public int id { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.RouteData.Values["id"] == null)
            {
                Response.Redirect("/error");
            }
            else
            {
                openChangeAvaModal.Visible = false;
                int fixId;
                bool tryParse = int.TryParse(Page.RouteData.Values["id"].ToString(), out fixId);
                if (tryParse && fixId > 0)
                {
                    id = fixId;
                    Page.Title = "Profile";

                    getUserData();
                    validateAuthorise();
                }
                else
                {
                    Response.Redirect("/error");
                }
            }
        }

        // check current user have authorise to make change to this profile
        private bool validateAuthorise()
        {
            rateDiv.Visible = false;
            settingIcon.Visible = false;
            User currentUser = Session["authenUser"] as User;
            if (currentUser != null && currentUser.ID == (id))
            {
                openChangeAvaModal.Visible = true;
                settingIcon.Visible = true;
                //disable rateDiv
                return true;
            }
            else if (currentUser != null && currentUser.ID != (id))
            {
                rateDiv.Visible = true;
            }
            return false;
        }

        // get user data
        private void getUserData()
        {
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(connStr);
                String query = "Select username, fullname, address, description,joined, rate, rate_numbers, phoneNumber from users where id = @id and active = 1";
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.Add(new SqlParameter("@id", id));

                connection.Open();
                command.ExecuteNonQuery();
                SqlDataReader reader = command.ExecuteReader();
                // check if user available or not
                if (reader.HasRows)
                {
                    getUserAvatar();
                    getUserTotalProduct();
                    while (reader.Read())
                    {

                        //get rate data

                        rateScore = int.Parse(reader["rate"].ToString());
                        rateNumber = int.Parse(reader["rate_numbers"].ToString());

                        // score 
                        ratedLabel.InnerHtml = (rateNumber != 0) ? (rateScore / rateNumber + " / 5") : ("N/A");
                        string ss = (rateNumber != 0) ? (rateNumber + "") : (rateNumber + "");
                        ratedLabel.Attributes["title"] = "By : " + ss;

                        // other data

                        string username = reader["username"].ToString();
                        usernameLabel.InnerHtml = username;

                        string fullname = (!string.IsNullOrEmpty(reader["fullname"].ToString())) ?
                            reader["fullname"].ToString() : ("*This user didn't provide fullname yet");
                        fullnameLabel.InnerHtml = fullname;

                        joinedDateLabel.InnerHtml = Convert.ToDateTime(reader["joined"].ToString()).ToString("yyyy-MM-dd");

                        addressLabel.InnerHtml = (!string.IsNullOrEmpty(reader["address"].ToString())) ?
                            reader["address"].ToString() : ("No location provide");

                        description.InnerHtml = (!string.IsNullOrEmpty(reader["description"].ToString())) ?
                            reader["description"].ToString() : ("Hii");

                        phoneNumber.InnerHtml = (!string.IsNullOrEmpty(reader["phoneNumber"].ToString())) ?
                            reader["phoneNumber"].ToString() : ("*No phonenumber provide");

                    }
                }
                else
                {
                    Response.Redirect("/error?message=User not found");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                connection.Close();
            }
        }

        // get user avatar
        private void getUserAvatar()
        {
            string url = "/user/avatar/" + id;
            //user avatar
            bgAvatar.Attributes["style"] = "background-image: url(" + url + ")";
        }


        //get user total product
        private void getUserTotalProduct()
        {
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(connStr);
                String query = "SELECT Count([product_id]) as Total FROM [dbo].[products] where createdBy = @id";
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.Add(new SqlParameter("@id", id));

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        productLabel.InnerHtml = reader["Total"].ToString();
                    }
                }
                else
                {
                    productLabel.InnerHtml = reader["Total"].ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                connection.Close();
            }
        }

        protected void changeAvaButton_Click(object sender, EventArgs e)
        {
            changeAvatar();
        }

        private void changeAvatar()
        {
            if (validateAuthorise())
            {
                SqlConnection connection = null;
                try
                {
                    connection = new SqlConnection(connStr);
                    String query = "UPDATE [dbo].[users] SET [avatar] = @avatar ,  [contentType] = @contentType WHERE id = @id";
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();

                    command.Parameters.Add(new SqlParameter("@id", id));
                    //get image data
                    // The byte[] to save the data in
                    byte[] avatar = avatarFile.FileBytes;
                    command.Parameters.Add(new SqlParameter("@avatar", avatar));
                    string contentType = Path.GetExtension(avatarFile.FileName);
                    command.Parameters.Add(new SqlParameter("@contentType", contentType));

                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    connection.Close();
                }

            }
        }

        protected void rateBtn_Click(object sender, EventArgs e)
        {
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(connStr);
                String query = "UPDATE [dbo].[users] SET [rate] = @rate ,  [rate_numbers] = @rate_number WHERE id = @id";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();

                command.Parameters.Add(new SqlParameter("@id", id));
                command.Parameters.Add(new SqlParameter("@rate_number", rateNumber + 1));
                command.Parameters.Add(new SqlParameter("@rate", rateScore + int.Parse(score.Text)));
                command.ExecuteNonQuery();
                score.Text = "";
                Page.Response.Redirect(Page.Request.Url.ToString(), true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
            }
        }


        public List<Product> getUserProductPerPage()
        {
            SqlConnection connection = new SqlConnection(connStr);
            try
            {

                string page = "";
                if (string.IsNullOrEmpty(Request.QueryString["page"]))
                {
                    page = "1";
                }
                else
                {
                    page = Request.QueryString["page"];
                }
                int pageIndex = int.Parse(page);

                int pageStartIndex = pageSize * (pageIndex - 1) + 1;
                int pageEndIndex = pageSize * (pageIndex);

                List<Product> products = new List<Product>();

                string query = @"Select * from (

                        Select ROW_NUMBER() over (order by products.viewNumber desc) as rn ,
                        [product_id]
                              ,[product_name]
                              ,[price]
                          FROM [dbo].[products]
                          where createdBy = " + id +
                          @") as x
                                 Where rn between @start and @end
                             ";


                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();

                command.Parameters.Add(new SqlParameter("@start", pageStartIndex));
                command.Parameters.Add(new SqlParameter("@end", pageEndIndex));

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Product product = new Product();
                    product.id = reader["product_id"].ToString();
                    product.productName = reader["product_name"].ToString();
                    product.price = Convert.ToDouble(reader["price"].ToString());
                    products.Add(product);
                }

                return products;

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                connection.Close();
            }

        }

        public string generatePagination()
        {
            try
            {
                string page = "";
                if (string.IsNullOrEmpty(Request.QueryString["page"]))
                {
                    page = "1";
                }
                else
                {
                    page = Request.QueryString["page"];
                }
                int pageIndex = int.Parse(page);

                int gap = 2;
                int totalPage = getTotalUserProduct();
                int maxPage = totalPage / pageSize + (totalPage % pageSize == 0 ? 0 : 1);

                if (maxPage == 1)
                {
                    return "";
                }

                return Pagination.Pagger.generate(pageIndex, gap, maxPage, "/user/detail", false);
            }
            catch (Exception)
            {
                throw;
            }

        }


        public int getTotalUserProduct()
        {
            SqlConnection connection = new SqlConnection(connStr);
            try
            {
                string query = @"SELECT Count([products].product_id) as total
                      FROM [dbo].[products] where createdBy =  " + id;
                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    return int.Parse(reader["total"].ToString());
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                connection.Close();
            }
            return 0;
        }


    }
}