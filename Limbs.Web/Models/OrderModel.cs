﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Limbs.Web.Models
{
    /*public class OrderModel
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Person who request the order
        /// </summary>
        public UserModel OrderRequestor { get; set; }

        /// <summary>
        /// Person who will use the order
        /// </summary>
        public UserModel OrderUser { get; set; }

        public virtual ICollection<AccessoryModel> Type { get; set; }

        public ProductModel Product { get; set; }
    }
    */

    public class ProductModel
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string Thumbnail { get; set; }

        public int Version { get; set; }

        public virtual ICollection<FileModel> Files { get; set; }

        public bool IsRightHand { get; set; }

    }

    public class FileModel
    {
        [Key]
        public int Id { get; set; }

        public string Url { get; set; }

        public string Descripcion { get; set; }

        public virtual ICollection<FileModel> Products { get; set; }

    }

    public class AccessoryModel
    {
        public AccessoryModel()
        {
            Color = new HashSet<Color>();
        }

        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Price { get; set; }

        public string ImageUrl { get; set; }

        public virtual ICollection<Color> Color { get; set; }
    }

    public class Color
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public virtual AccessoryModel Accessory { get; set; }
    }

    public class UserModel
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public DateTime Birth { get; set; }

        public string Gender { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public string Address { get; set; }

        public long Lat { get; set; }

        public long Long { get; set; }
    }
}