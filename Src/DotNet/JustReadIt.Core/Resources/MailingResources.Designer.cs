﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18051
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace JustReadIt.Core.Resources {
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
    internal class MailingResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal MailingResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("JustReadIt.Core.Resources.MailingResources", typeof(MailingResources).Assembly);
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
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;p&gt;
        ///  Welcome!
        ///&lt;/p&gt;
        ///
        ///&lt;p&gt;
        ///  Your &lt;a href=&quot;http://www.justread.it&quot;&gt;justRead.it&lt;/a&gt; account has been created.
        ///&lt;/p&gt;
        ///
        ///&lt;p&gt;
        ///  Please click the following link to verify your e-mail address:
        ///&lt;/p&gt;
        ///
        ///&lt;p style=&quot;font-weight: bold;&quot;&gt;
        ///  &lt;a href=&quot;http://www.justread.it/Account/VerifyEmail?token=${EmailVerificationToken}&quot;&gt;http://www.justread.it/Account/VerifyEmail?token=${EmailVerificationToken}&lt;/a&gt;
        ///&lt;/p&gt;
        ///
        ///&lt;p&gt;
        ///  Once you&apos;ve done that, you can &lt;a href=&quot;http://www.justread.it/Account/SignIn&quot;&gt;sign in to justRead [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string MailBodyHtml_VerificationEmailBodyHtml {
            get {
                return ResourceManager.GetString("MailBodyHtml_VerificationEmailBodyHtml", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Your justRead.it account has been created.
        /// </summary>
        internal static string MailSubject_VerificationEmail {
            get {
                return ResourceManager.GetString("MailSubject_VerificationEmail", resourceCulture);
            }
        }
    }
}
