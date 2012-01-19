// 
// Main.cs
//  
// Author:
//       Rasmus Pedersen <rasmus@akvaservice.dk>
// 
// Copyright (c) 2010 Rasmus Pedersen
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using Mono.CSharp;
using System.Threading;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using SNDK.DBI;
using SNDK.Enums;

using System.Reflection;

using SorentoLib;
//using sCMS;
//using sBlog;
//using Akvabase;

namespace Test
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Evaluator.Compile ("using SorentoLib; Test (object lala) {};");


//			SorentoLib.Services.Database.Connection = new Connection (Toolbox.Enums.DatabaseConnector.Mysql,
//			                                                            "10.0.0.40",
//			                                                            "sorentotest.sundown.dk",
//			                                                            "sorentotest",
//			                                                            "scumbukket",
//			                                                            true);

//			SorentoLib.Services.Addins.Initialize ();
//			SorentoLib.Services.Config.Initialize ();
//			SorentoLib.Services.Database.Initalize ();

//			SorentoLib.Services.Snapshot.Take ();

//			foreach (Entry entry in Entry.List ())
//			{
//				Console.WriteLine (entry.Title);
//			}

//			sBlog.Blog blog1 = new sBlog.Blog ();
//			blog1.Name = "Test Blog";
//			blog1.Save ();
//
//			sBlog.Entry entry1 = new sBlog.Entry (blog1);
//			entry1.Title = "Test entry #1";
//			entry1.Save ();
//
//			sBlog.Comment comment1 = new sBlog.Comment (entry1);
//			comment1.Name = "Rasmus Pedersen";
//			comment1.Content = "Bla bla bla bla bla";
//			comment1.Save ();
//
//			sBlog.Comment comment2 = new sBlog.Comment (entry1);
//			comment2.Name = "Ina Kristiansen";
//			comment2.Content = "blu blu blu blu blu";
//			comment2.Save ();

//			foreach (sBlog.Entry entry in blog1.Entries)
//			{
//				Console.WriteLine (entry.Title);
//
//				foreach (sBlog.Comment comment in entry.Comments)
//				{
//					Console.WriteLine ("\t"+ comment.Name);
//					Console.WriteLine ("\t\t"+ comment.Content);
//				}
//			}
//
//
//			sBlog.Blog.Delete (blog1.Id);

			Environment.Exit (0);

//			foreach (SorentoLib.Services.Snapshot snapshot in SorentoLib.Services.Snapshot.List())
//			{
//				Console.WriteLine (snapshot.Date);
//				SorentoLib.Services.Snapshot.Develop (snapshot);
//				SorentoLib.Services.Snapshot.Delete (snapshot.Id);
//			}


//			SorentoLib.Services.Snapshot.Develop ("snapshot_03-06-2011_08-09-07");

//			sCMS.Collection collection1 = sCMS.Collection.Load (new Guid ("7ff0a929-c73e-4f42-a855-070ae6c07b87"));

//			collection1.ToAjaxItem ();

//			foreach (Collection collection in sCMS.Collection.List ())
//			{
//				Console.WriteLine (collection.Name);
//
//			}
//
//			sCMS.CollectionSchema schema1 = new sCMS.CollectionSchema ("Test Schema");
//			schema1.Save ();
//
//			sCMS.Collection collection1 = new sCMS.Collection ("Test Collection", schema1);
//			collection1.Save ();
//
//			sCMS.Collection collection2 = sCMS.Collection.Load (collection1.Id);
//			Console.WriteLine (collection2.Schema.Name +" "+ collection2.Name);
//
//			sCMS.CollectionSchema.Delete (schema1.Id);
//
//			SorentoLib.Ajax.Respons r1 = new SorentoLib.Ajax.Respons ();

//			sCMS.Template t1 = sCMS.Template.Load (new Guid ("f86765b0-42f6-49e3-926f-fdcfbcdd4af2"));
//			t1.Save ();
//			t1.Fields[0].Options.Add ("transformations", "BLA BLA");
//
//			t1.ToAjaxRespons (r1);
//
//			Console.WriteLine (r1.Data.Count);
//			foreach (string key in r1.Data.Keys)
//			{
//				Console.WriteLine (key);
//			}
//
//
//			string ajax1 = r1.WriteResponse ();
//
//			SorentoLib.Ajax.Request r2 = new SorentoLib.Ajax.Request (ajax1);
//
//			Console.WriteLine (r2.Data.Count);
//
////			Console.WriteLine (r2.Data["options"]);
//
//			foreach (string key in r2.Data.Keys)
//			{
//				Console.WriteLine (key);
//			}
//
//			Field field = Field.FromAjaxItem (((List<Hashtable>)r2.Data["fields"])[0]);
//			Console.WriteLine (field.Options.Count);
//			Console.WriteLine (field.Options["transformations"]);


//			Hashtable test = (Hashtable)r2.Data["options"];

//			Console.WriteLine (test["transformations"]);

//			Console.WriteLine (r1.WriteResponse ());




//			MediaTransformation t1 = new MediaTransformation (SorentoLib.Enums.MediaTransformationType.Image);
//			t1.Title = "Forside rotator";
//			t1.Script = "bla bla bla";
//			t1.Save ();

//			Media media1 = Media.Load (new Guid ("fd913500-af7c-4026-9d5e-5061996b87e4"));

//			Media media1 = new Media ("content/image1", "/home/sundown/Wallpapers/05.jpg", false);
//			media1.Status = SorentoLib.Enums.MediaStatus.PublicTemporary;
//			media1.Save ();



//			MediaTransformation t1 = new MediaTransformation (SorentoLib.Enums.MediaTransformationType.Image);
//			t1.Title = "Test 1";

//			t1.MimeTypes.Add ("image/jpeg");

//			t1.MimeTypes.Add ("jpeg/image");
//			t1.MimeTypes.Add ("gif/image");


//			string xml = string.Empty;
//			foreach (string line in Toolbox.IO.ReadTextFile ("/home/sundown/Skrivebord/gfxtest/g-test/thumbnails.xml", Encoding.UTF8))
//			{
//				xml += line;
//			}
//
//			t1.Script = xml;
//
//			Console.WriteLine (media1.FileName);
//			Media media2 = media1.Clone ("thumbs/"+ media1.FileName);
//			Media media2 = media1.Clone (media1.Path);
//			t1.Transform (media2);

//			t1.Transform (media1);
//			Console.WriteLine ("dfdf");
//			t1.Save ();
//			MediaTransformation.Delete (t1.Id);

//			Toolbox.Graphics.ParseJob (xml, "/home/sundown/Skrivebord/gfxtest/g-test/input.jpg", "/home/sundown/Skrivebord/output");


			Environment.Exit (0);
//			Toolbox.Graphics.ParseJob ()


//			foreach (MediaTransformation t in MediaTransformation.List ())
//			{
//				Console.WriteLine (t.Type);
//
//				Hashtable item = t.ToAjaxItem ();
//
//				MediaTransformation.Delete (t.Id);

//				MediaTransformation t2 = MediaTransformation.FromAjaxItem (item);
//				t2.Save ();

//				Console.WriteLine (t2.Type);
//
//				MediaTransformation.Delete (t2.Id);
//			}

			Environment.Exit (0);



//			Autoform.Form test = new Autoform.Form ();
//			test.Title = "Test";
//			test.Save ();

//			List<string> test = new List<string> ();
//			test.Add ("test1");
//			test.Add ("test2");
//
//			object test2 = test;
//
//			MethodInfo methodInfo = typeof(SorentoLib.Render.Variables).GetMethod("ConvertToListObjectNew", System.Reflection.BindingFlags.Static | BindingFlags.Public);
//			MethodInfo genericMethodInfo = methodInfo.MakeGenericMethod(new Type[] { test2.GetType ().GetGenericArguments()[0] });
//			List<object> returnValue = (List<object>)genericMethodInfo.Invoke(null, new object[] { test2 });
//
//			foreach (object item in returnValue)
//			{
//				Console.WriteLine (item);
//			}



//			MethodInfo method = typeof (SorentoLib.Render.Variables).GetMethod ("ConvertToListObject");
//			MethodInfo generic = method.MakeGenericMethod (typeof(string));
//			generic.Invoke ()


//MethodInfo method = typeof(Sample).GetMethod("GenericMethod");
//MethodInfo generic = method.MakeGenericMethod(myType);
//generic.Invoke(this, null);

//			GenericInvoker invoker;
//
//    // invoke method that returns void
//    invoker = DynamicMethods.GenericMethodInvokerMethod(typeof(SampleClass), "Test",
//        new Type[] { typeof(string) });
//    ShowResult(invoker(instance, "this is a tests"));
//		SorentoLib.Render.Variables.ConvertToListObject<string> ((List<string>) test);
//

			//
//			object test2 = test;
//
//			foreach (object item in (test2.GetType ())Convert.ChangeType (test2, test2.GetType ()))
//			{
//
//			}

//			Console.WriteLine (test2.GetType ());



//			object test3 = test;
//
//			List<object> test4 = (List<object>)test3;
//
//			foreach (object test2 in test)
//			{
//
//			}
//
//			test.c

//			SorentoLib.Render.Variables.ConvertToListTest (test);

//			Global.List ();
//			Global global = new Global (sCMS.Enums.FieldType.Text);
//			global.Save ();



			Environment.Exit (0);




			SorentoLib.Services.Config.Initialize ();
			SorentoLib.Services.Database.Initalize ();

//			sCMS.Stylesheet css = sCMS.Stylesheet.Load ("forside");
//			css.Save ();

//			sCMS.Stylesheet css = new sCMS.Stylesheet ("forside");
//			css.Content += "Test";
//			Console.WriteLine (css.Title);
//			css.Save ();

//			foreach (Stylesheet css in sCMS.Stylesheet.List ())
//			{
//				Console.WriteLine (css.Title);
//			}


//			User user = new User ();
//			user.Username = "admin";
//
//			user.Email = "thomas@rsport.dk";
//			user.Password = "tubeklum";
//			Console.WriteLine (user.Password);
//			user.Save ();

			Environment.Exit (0);

//			Hashtable test = new Hashtable ();
//			Console.WriteLine (test.GetType ().FullName);


//			List<sCMS.Page> pages = new List<sCMS.Page> ();
//			pages.Add (sCMS.Page.Load ("behandlingsform"));
//			pages.Add (sCMS.Page.Load ("behandlingsform2"));
//			pages.Add (sCMS.Page.Load ("behandlingsform3"));
//			pages.Add (sCMS.Page.Load ("behandlingsform4"));
////
//			sCMS.Page pagepage = sCMS.Page.Load ("behandlingsformer");
//			pagepage.Set (pagepage.Template.GetField ("Features").Id, pages);
//
//
//			foreach (sCMS.Page p in (List<sCMS.Page>)pagepage.Get ("Features"))
//			{
//				Console.WriteLine (p.Name);
//			}

//			pagepage.Save ();

//			Environment.Exit (0);


//			SorentoLib.Media media1 = Media.Load (new Guid ("83c5c2c0-fc35-4a0c-a5a1-f0095f175e8c"));
//			SorentoLib.Media media2 = Media.Load (new Guid ("7336a8eb-aaca-4b1b-a497-2429b6516e8c"));
//			SorentoLib.Media media3 = Media.Load (new Guid ("1f849d6c-08de-4f3a-9884-ba1bd26773e0"));

//			List<SorentoLib.Media> medialist = new List<SorentoLib.Media> ();
//			medialist.Add (Media.Load (new Guid ("83c5c2c0-fc35-4a0c-a5a1-f0095f175e8c")));
//			medialist.Add (Media.Load (new Guid ("7336a8eb-aaca-4b1b-a497-2429b6516e8c")));
//			medialist.Add (Media.Load (new Guid ("1f849d6c-08de-4f3a-9884-ba1bd26773e0")));
//
//			sCMS.Page pagetest = sCMS.Page.Load ("prisliste");
//			pagetest.Set (pagetest.Template.GetField ("pictures").Id, medialist);

//			Hashtable hash = new Hashtable ();
//			hash.Add ("Ting 1", "100");
//			hash.Add ("Ting 2", "200");
//			hash.Add ("Ting 3", "300");
//			hash.Add ("Ting 4", "400");
//			hash.Add ("Bla bla bla bla", "");

//			pagetest.Set (pagetest.Template.GetField ("prices1").Id, hash);

//			pagetest.Save ();

//			Console.WriteLine (pagetest.Get ("prices1").GetType ().FullName);
//
////			Hashtable test2 = (Hashtable)pagetest.Get ("prices1");
//
//
//			foreach (string key in ((System.Collections.Hashtable)pagetest.Get ("prices1")).Keys)
//			{
//				Console.WriteLine (key);
//			}
//


//
//			List<SorentoLib.Media> medialist2 = (List<SorentoLib.Media>)pagetest.Get (pagetest.Template.GetField ("pictures").Id);
//
//			foreach (SorentoLib.Media bla in medialist2)
//			{
//				Console.WriteLine (bla.Id);
//			}
//
//			pagetest.Save ();
//
//			Environment.Exit (0);
//			Guid id = new Guid ("e3b2eaa6-c499-4b9a-a63b-0d0448867456"); // MainTemplate
//			Guid id = new Guid ("185f0e7f-2f65-425f-a956-b97540b408c2"); // PrislisteTemplate
//			Guid id = new Guid ("945da3aa-1ae8-43b0-a9ce-67fc7c0aa0aa"); // ForsideTemplate
//			Guid id = new Guid ("44491850-35b0-4f20-83ad-ec651485de06"); // KontaktTemplate
//			Guid id = new Guid ("1b0b84a5-aa75-4048-aa16-e92179f948f0"); // BehandlingsFormTemplate
//			Guid id = new Guid ("4cd22a5f-6e6c-498a-8668-3b7dc521c285"); // BehandlingsFormerTemplate

//			Template template = Template.Load (id);

//			Template template = new Template ();
//			template.Title = "BehandlingsFormerTemplate";
//			template.Parent = template2;

//			sCMS.Page page = new sCMS.Page (template);
//			page.Name = "behandlingsform4";
//			page.Save ();



//PRICES1 0
//TITLE 2
//SEPERATORTITLE 3
//TOPTEXT 5
//TOPHEADLINE 6
//PRICESHEADLINE 10
//BOTTOMHEADLINE 20
//BOTTOMTEXT 21
//SEPERATOREXTRA 90
//FOOTERTEXT 91
//SEPERATORCSS 100
//CSS 101

//		Console.WriteLine (template.Title);

//			template.Content = string.Empty;
//			foreach (string line in Toolbox.IO.ReadTextFile ("/home/sundown/Skrivebord/rsport-kontakt-template", Encoding.UTF8))
//			foreach (string line in Toolbox.IO.ReadTextFile ("/home/sundown/Skrivebord/rsport-prisliste-template", Encoding.UTF8))
//			foreach (string line in Toolbox.IO.ReadTextFile ("/home/sundown/Skrivebord/rsport-main-template", Encoding.UTF8))
//			foreach (string line in Toolbox.IO.ReadTextFile ("/home/sundown/Skrivebord/rsport-forside-template", Encoding.UTF8))
//			foreach (string line in Toolbox.IO.ReadTextFile ("/home/sundown/Skrivebord/rsport-behandlingsform-template", Encoding.UTF8))
//			foreach (string line in Toolbox.IO.ReadTextFile ("/home/sundown/Skrivebord/rsport-behandlingsformer-template", Encoding.UTF8))
//			{
//				template.Content += line +"\n";;
//				Console.WriteLine (line);
//			}

//			template.RemoveField ("telefon");
//			template.RemoveField ("email");

//			template.GetField ("headline").Sort = 10;
//			template.GetField ("headlinetext").Sort = 11;
//
//			template.GetField ("seperator1").Sort = 19;
//			template.GetField ("maintext").Sort = 20;
//			template.GetField ("phone").Sort = 21;
//
//			template.GetField ("seperator2").Sort = 29;
//
//			template.GetField ("mainpicture").Sort = 30;
//			template.GetField ("pictures").Sort = 31;
//			template.GetField ("pictures").Sort = 32;


//			template.AddField (new Field (sCMS.Enums.FieldType.Seperator, "Seperator1"));
//			template.AddField (new Field (sCMS.Enums.FieldType.ListHashtable, "Prices1"));
//			template.AddField (new Field (sCMS.Enums.FieldType.Text, "address"));
//			template.AddField (new Field (sCMS.Enums.FieldType.Text, "phone"));
//			template.AddField (new Field (sCMS.Enums.FieldType.Text, "email"));


//			foreach (Field testfield in template.Fields)
//			{
//				Console.WriteLine (testfield.Name +" "+ testfield.Sort);
//			}
//
//			template.Save ();


//
			Environment.Exit (0);

//			Template template1 = new Template ();
//			Template template2 = Template.Load (new Guid ("945da3aa-1ae8-43b0-a9ce-67fc7c0aa0aa"));
//			Template template1 = Template.Load (new Guid ("e3b2eaa6-c499-4b9a-a63b-0d0448867456"));


//			template1.Parent = template2;
//			template1.RemoveField ("pagetitle");

//			template1.AddField (new Field (sCMS.Enums.FieldType.String, "Title"));
//			template1.AddField (new Field (sCMS.Enums.FieldType.String, "FooterText"));

//			template1.AddField (new Field (sCMS.Enums.FieldType.String, "TopHeadline"));
//			template1.AddField (new Field (sCMS.Enums.FieldType.Text, "TopText"));
//			template1.AddField (new Field (sCMS.Enums.FieldType.String, "PricesHeadline"));
//			template1.AddField (new Field (sCMS.Enums.FieldType.String, "BottomHeadline"));
//			template1.AddField (new Field (sCMS.Enums.FieldType.Text, "BottomText"));

//			template1.RemoveField ("Feature3Image");



//			Console.WriteLine (template1.Title);
//			template1.Title = "MainTemplate";

//			template1.Content = string.Empty;
//			foreach (string line in Toolbox.IO.ReadTextFile ("/home/sundown/Skrivebord/rsport-prisliste-template", Encoding.UTF8))
//			foreach (string line in Toolbox.IO.ReadTextFile ("/home/sundown/Skrivebord/rsport-forside-template", Encoding.UTF8))
//			foreach (string line in Toolbox.IO.ReadTextFile ("/home/sundown/Skrivebord/rsport-main-template", Encoding.UTF8))
//			{
//				template1.Content += line +"\n";;
//			}

//			template1.Save ();

//			Environment.Exit (0);




//			template1.GetField ("Title").Sort = 1;
//			template1.GetField ("FooterText").Sort = 50;
//
//			template1.GetField ("TopHeadline").Sort = 6;
//			template1.GetField ("TopText").Sort = 5;
//
//			template1.GetField ("PricesHeadline").Sort = 10;
//
//			template1.GetField ("BottomHeadline").Sort = 20;
//			template1.GetField ("BottomText").Sort = 21;

//			template1.GetField ("Feature1Image").Sort = 13;
//
//			template1.GetField ("Feature2Headline").Sort = 14;
//			template1.GetField ("Feature2Text").Sort = 15;
//			template1.GetField ("Feature2Link").Sort = 16;
//			template1.GetField ("Feature2Image").Sort = 17;
//
//			template1.GetField ("Feature3Headline").Sort = 18;
//			template1.GetField ("Feature3Text").Sort = 19;
//			template1.GetField ("Feature3Link").Sort = 20;
//			template1.GetField ("Feature3Image").Sort = 21;
//
//			foreach (Field testfield in template1.Fields)
//			{
//				Console.WriteLine (testfield.Sort);
//			}



//			template1.AddField (new Field (sCMS.Enums.FieldType.String, "Feature1Headline"));
//			template1.AddField (new Field (sCMS.Enums.FieldType.String, "Feature2Headline"));
//			template1.AddField (new Field (sCMS.Enums.FieldType.String, "Feature3Headline"));
//			template1.AddField (new Field (sCMS.Enums.FieldType.String, "Feature1Link"));
//			template1.AddField (new Field (sCMS.Enums.FieldType.String, "Feature2Link"));
//			template1.AddField (new Field (sCMS.Enums.FieldType.String, "Feature3Link"));
//			template1.AddField (new Field (sCMS.Enums.FieldType.Text, "Feature3Text"));
//			template1.AddField (new Field (sCMS.Enums.FieldType.Text, "Feature1Text"));
//			template1.AddField (new Field (sCMS.Enums.FieldType.Text, "Feature2Text"));

//			template1.AddField (new Field (sCMS.Enums.FieldType.String, "FooterText"));

//			template1.GetField ("FooterText").Type = sCMS.Enums.FieldType.String;

//			template1.RemoveField ("feature3text_3");

//			template1.Save ();


//PageTitle
//FooterText

//WelcomeHeadline
//WelcomeText

//Feature1Headline
//Feature2Headline
//Feature3Headling

//Feature1Text
//Feature3Text
//				Feature2Text

//Feature1Link
//Feature2Link
//Feature3Link


Environment.Exit (0);



//Environment.Exit (0);

//			Content content2 = Content.Load ("Behandlingsformer");
//
//			Console.WriteLine (content1.Name);
//			Console.WriteLine (content2.Name);
//			Console.WriteLine (content2.Template.Title);
//			Console.WriteLine (content2.Get ("headline"));
//			Console.WriteLine (content2.Get ("subheadline"));


//			Console.WriteLine (content1.Get ());
//			Console.WriteLine (content1.Get ());
//			Console.WriteLine (content1.Get ());

//			Console.WriteLine (content2.Get (data3).GetType());


//			Content content2 = Content.Load (content1.Id);
//
//			Console.WriteLine (content2.Title);
//			Console.WriteLine (content2.Template.Title);

//			Template.Delete (temp1.Id);
//			Content.Delete (content1.Id);
//			Content.Delete (content2.Id);



//			Template temp1 = new Template ();
//			temp1.Title = "Main Template";
//			temp1.Content = string.Empty;
//			temp1.Content += "<body>\n";
//			temp1.Content += " <div>\n";
//			temp1.Content += "  [CHILD_TEMPLATE_PLACEHOLDER]\n";
//			temp1.Content += " </div>\n";
//			temp1.Content += "</body>\n";
//			temp1.Save ();
//
//			Template temp2 = new Template ();
//			temp2.Title = "Sub Template";
//			temp2.Content = string.Empty;
//			temp2.Content += " <span>\n";
//			temp2.Content += " [CHILD_TEMPLATE_PLACEHOLDER]\n";
//			temp2.Content += " </span>\n";
//			temp2.Parent = temp1;
//			temp2.Save ();
//
//			Template temp3 = new Template ();
//			temp3.Title = "Sub Template";
//			temp3.Content = string.Empty;
//			temp3.Content += " TEST";
//			temp3.Parent = temp1;

//			DataField field1 = new DataField ();
//			field1.

//			temp3.DataFields.Add ()

//			Console.WriteLine (temp3.Build ());

//			Template.Delete (temp1.Id);
//			Template.Delete (temp2.Id);


//			sCMS.Template test = sCMS.Template.Load (new Guid ("	3c002878-e830-43b4-8df0-588b48980360"));
//			Console.WriteLine ("Title: "+ test.Title);
//			Console.WriteLine ("Content: "+ test.Content);
//			Console.WriteLine ("Master: "+ test.Master);

//			sCMS.Template test = new sCMS.Template ();
//			test.Title = "The second test template";
//			test.Master = false;
//			test.Content = "HTML HTML HTML";
//			test.Save ();


//			foreach (Template template in Template.List ())
//			{
//				Console.WriteLine (template.Title);
//			}





//			Media media1 = new Media ("TestFiles/File1.data", "/home/sundown/Skrivebord/testdata.data", false);
//			media1.Status = SorentoLib.Enums.MediaStatus.Temporary;
//			media1.Save ();
//
//			Thread.Sleep (2000);

//			Media.Delete (media1.Id);
//			media1.Save ();

//			Media media2 = Media.Load (media1.Id);
//			media2.Status = SorentoLib.Enums.MediaStatus.Public;
//			media2.Save ();

//			Thread.Sleep (5000);
//			media1.Status = SorentoLib.Enums.MediaStatus.Public;
//			media1.Save ();
//
//			Thread.Sleep (5000);
//			media1.Save ();
//						media1.Status = SorentoLib.Enums.MediaStatus.Restricted;


//			SorentoLib.Services.Database.Connection.Connect ();
//
//			SorentoLib.Services.Config.Set ("database", "prefix", "sorento_");
//			SorentoLib.Services.Config.Set ("core", "pathtmp", "./tmp/");
//			SorentoLib.Services.Config.Set ("core", "pathmedia", "./media/");
//			SorentoLib.Services.Config.Set ("core", "pathpublicmedia", "../html/media/");

//			SorentoLib.Media media = SorentoLib.Media.Load (new Guid ("b5d21559-096d-40f9-85a0-51cfef939eec"));
//
//			Console.WriteLine (media.Path);
//			Console.WriteLine (media.Size);


//

//			SorentoLib.Services.Config.Initialize ();
////
//			SorentoLib.Services.Config.Set ("database", "hostname", "10.0.0.40");
//
//			Thread.Sleep (7000);
//			Console.WriteLine ("7");
//			SorentoLib.Services.Config.Set ("database", "hostname", "10.0.0.10");
//
//
//			Thread.Sleep (10000);
//			Console.WriteLine ("10");
//
//			Thread.Sleep (15000);
//			Console.WriteLine ("15");
//
//
//			Thread.Sleep (20000);
//			Console.WriteLine ("20");
//
//			Thread.Sleep (25000);
//			Console.WriteLine ("25");
//
//
//			Thread.Sleep (50000);
//
//			Environment.Exit (0);

//			SorentoLib.Media media = new SorentoLib.Media ("avatars/test.jpg", Toolbox.IO.FileToByteArray ("/home/sundown/Skrivebord/test.jpg"));
//			SorentoLib.Media media = new SorentoLib.Media ("avatars/me.jpg", "/home/sundown/Skrivebord/bigbill.jpg", false);
//			SorentoLib.Media media = new SorentoLib.Media ("fromurl/test1.jpg", "http://gfx.newz.zfour.dk/57/15957-250x161crop0.jpg");

//			media.Status = SorentoLib.Enums.MediaStatus.Public;
//			media.AccessLevel = SorentoLib.Enums.Accesslevel.Administrator;
//
//			media.UserGroups.Add (new SorentoLib.UserGroup ());
//			Console.WriteLine (media.UserGroups.Count);
//			media.Save ();
//
//			SorentoLib.Media media2 = Media.Load (media.Id);
//			Console.WriteLine (media2.UserGroups.Count);
//			Thread.Sleep (5000);
//			media.Path = "fromurl/test2.jpg";
//			media.Save ();
//			Thread.Sleep (5000);
//			media.Status = SorentoLib.Enums.MediaStatus.Restricted;
//			media.Save ();
//			Thread.Sleep (5000);
//			media.Status = SorentoLib.Enums.MediaStatus.Temporary;
//			media.Save ();
//			Thread.Sleep (5000);
//			media.Status = SorentoLib.Enums.MediaStatus.Restricted;
//			media.Save ();
//			Thread.Sleep (5000);
//			media.Status = SorentoLib.Enums.MediaStatus.Public;
//			media.Save ();
//			Thread.Sleep (5000);
//			media.Path = "fromurl/test.jpg";
//			media.Save ();
//			Thread.Sleep (5000);
//			media.Delete ();
//			Console.WriteLine ("done");

//			Thread.Sleep (2000);

//			media.Delete ();
//			Console.WriteLine (media.Directory);


//			SorentoLib.Media media = new SorentoLib.Media ();
//			media.Path = "avatars/";
//			media.Filename = "test.jpg";
//			media.Status = SorentoLib.Enums.MediaStatus.Public;
//			media.Save ();
//			media.Delete ();

//			SorentoLib.Media media = SorentoLib.Services.MediaLibrary.AddFromByteArray (Toolbox.IO.FileToByteArray ("/home/sundown/Skrivebord/test.jpg"), "avatars/test.jpg");
//			SorentoLib.Services.MediaLibrary


			

//			SorentoLib.Media media = SorentoLib.Media.CreateFromByteArray ();

//			Console.WriteLine (media.Filename);
//			Console.WriteLine (media.Mimetype);


//			SorentoLib.Services.Crypto.Initalize ();

//			string data = "1e92ecf8e9dcc02cf3a828cbeb19b5360e6a1b28815201777b445f1372d96c175ab6ba21bbcdc5f09efb785dedbd6f9997e68a5c99ed51cbf91977f49d99cb8dee7cbb2c577f66278908f62815a69d18f7f013df0c8355ca009759e7d1365ab4c696d36fc0a0022521d96d0df0852c9923d09ffb37b04e655a2575aba48629e4";
//			string tmp = SorentoLib.Tools.StringHelper.ASCIIBytesToString (SorentoLib.Services.Crypto.Decrypt (SorentoLib.Tools.StringHelper.HexStringToBytes (data)));
//
//			Console.WriteLine (tmp);


//			Console.WriteLine (SorentoLib.Tools.EncodeTo64(SorentoLib.Tools.Encrypt ("scumbukket")));
			//Console.WriteLine (SorentoLib.Tools.Decrypt ("V3URdasPAkq0G3GXzYJmLw=="));
//			0x57751175ab0f024ab41b7197cd82662f

//			SorentoLib.Services.Datastore.Set ("Test.test", "da", "Lalalalalala");
//			SorentoLib.Services.Datastore.Set ("akvabase.email.forgot.body", "da", "Lalalalalala");

//			Console.WriteLine (SorentoLib.Services.Datastore.Get ("akvabase.email.signup", "da"));

			///SorentoLib.Services.Datastore.Delete ("akvabase.email.signup", "da");

//			Akvabase.Profile profile = new Akvabase.Profile ();
//			profile.Firstname = "Rasmus";
//			profile.Lastname = "Pedersen";
//			profile.Email = "la@la.dk";
//			Console.WriteLine (profile.Save ());

//			SorentoLib.Ajax.Request request = new SorentoLib.Ajax.Request ("<variables><text><![CDATA[lalalla]]></text></variables>");

//			UserGroup usergroup = new UserGroup();
//			usergroup.Name = "Test Group";
//			usergroup.Accesslevel = SorentoLib.Enums.Accesslevel.Administrator;
//			usergroup.Save();
//
//			User user = new User();
//			user.Username = "Tester";
//			user.Password = "secret";
//			user.Realname = "Test Testesen";
//			user.Email = "test@test.dk";
//			user.UserGroups.Add(usergroup);
//			user.Save();
		}
	}
}
