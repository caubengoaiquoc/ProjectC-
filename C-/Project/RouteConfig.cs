﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace Project
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.Ignore("{resource}.axd/{*pathInfo}");

            //normal page
            routes.MapPageRoute("Default", "", "~/NormalPage/Home.aspx" , true);
            routes.MapPageRoute("Home", "home", "~/NormalPage/Home.aspx" , true);
            routes.MapPageRoute("Category", "category/{id}/{name}", "~/NormalPage/Categories.aspx", true);
            routes.MapPageRoute("Search", "search", "~/NormalPage/Search.aspx", true);

            //admin
            routes.MapPageRoute("AdminManage", "admin/manage", "~/NormalPage/Manage.aspx", true);

            //manage
            routes.MapPageRoute("ManageProduct", "user/product", "~/NormalPage/ManageProduct.aspx", true);
            routes.MapPageRoute("ProductOrder", "order/product/{id}", "~/NormalPage/ProductOrder.aspx", true);

            //order
            routes.MapPageRoute("Cart", "cart", "~/NormalPage/Cart.aspx", true);
            routes.MapPageRoute("WatchedProduct", "watched", "~/NormalPage/WatchedProduct.aspx", true);
            routes.MapPageRoute("OrderView", "order", "~/NormalPage/ViewOrder.aspx", true);


            //profile
            routes.MapPageRoute("Profile", "user/detail/{id}", "~/NormalPage/Profile.aspx", true);
            routes.MapPageRoute("avatar", "user/avatar/{id}", "~/NormalPage/PageImage.aspx", true);
            routes.MapPageRoute("Image", "image", "~/NormalPage/Image.aspx", true);
            routes.MapPageRoute("Setting", "user/setting", "~/NormalPage/Setting.aspx", true);

            //product
            routes.MapPageRoute("AddProduct", "product/add", "~/NormalPage/AddProduct.aspx", true);
            routes.MapPageRoute("Product Detail", "product/detail/{id}", "~/NormalPage/ProductDetail.aspx", true);
            routes.MapPageRoute("Product Edit", "product/edit/{id}", "~/NormalPage/EditProduct.aspx", true);

            //Authen page
            routes.MapPageRoute("Login", "login", "~/AuthenPage/Login.aspx");
            routes.MapPageRoute("Signup", "signup", "~/AuthenPage/Signup.aspx");

            //handle error
            routes.MapPageRoute("Error", "error", "~/NormalPage/ErrorPage.aspx");
        }
    }
}