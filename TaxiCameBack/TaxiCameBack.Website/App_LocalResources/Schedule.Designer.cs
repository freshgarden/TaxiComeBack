﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TaxiCameBack.Website.App_LocalResources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Schedule {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Schedule() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("TaxiCameBack.Website.App_LocalResources.Schedule", typeof(Schedule).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Tạo lịch trình thành công.
        /// </summary>
        public static string create_schedule_success {
            get {
                return ResourceManager.GetString("create_schedule_success", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Đăng ký lộ trình.
        /// </summary>
        public static string lbl_create_schedule {
            get {
                return ResourceManager.GetString("lbl_create_schedule", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Quản lý lộ trình.
        /// </summary>
        public static string lbl_manage_schedule {
            get {
                return ResourceManager.GetString("lbl_manage_schedule", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Lịch trình.
        /// </summary>
        public static string schedule_breadcrumb {
            get {
                return ResourceManager.GetString("schedule_breadcrumb", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Lịch trình.
        /// </summary>
        public static string title {
            get {
                return ResourceManager.GetString("title", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cập nhật lịch trình thành công.
        /// </summary>
        public static string update_schedule_success {
            get {
                return ResourceManager.GetString("update_schedule_success", resourceCulture);
            }
        }
    }
}
