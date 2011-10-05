// 
// Scripting.cs
//  
// Author:
//       Rasmus Pedersen <rasmus@akvaservice.dk>
// 
// Copyright (c) 2011 Rasmus Pedersen
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
using System.IO;
using System.Xml;
using System.Collections;

using System.Drawing;

using SNDK;

// TODO: special tags in parser.

namespace SorentoLib.Services
{
	public class Scripting
	{
		private static string ReplaceTags (string String, Hashtable Variables)
		{
			foreach (string key in Variables.Keys)
			{
				String = String.Replace ("%%"+ key.ToUpper () +"%%", (string)Variables[key]);
			}

			return String;
		}

		static public bool Parse (string Script, string PathSource, string PathDestination)
		{
			bool result = false;

			Hashtable variables = new Hashtable ();
			variables.Add ("sourcepath", PathSource);
			variables.Add ("sourcedirectoryname", System.IO.Path.GetDirectoryName (PathSource));
			variables.Add ("sourcefilename", System.IO.Path.GetFileName (PathSource));
			variables.Add ("sourcefilenamewithoutextension", System.IO.Path.GetFileNameWithoutExtension (PathSource));
			variables.Add ("sourceextension", System.IO.Path.GetExtension (PathSource));

			variables.Add ("destinationpath", PathDestination);
			variables.Add ("destinationdirectoryname", System.IO.Path.GetDirectoryName (PathSource));
			variables.Add ("destinationfilename", System.IO.Path.GetFileName (PathSource));
			variables.Add ("destinationfilenamewithoutextension", System.IO.Path.GetFileNameWithoutExtension (PathSource));
			variables.Add ("destinationextension", System.IO.Path.GetExtension (PathSource));

			variables.Add ("htmlroot", SorentoLib.Services.Config.Get<string> (Enums.ConfigKey.core_pathhtmlroot));


			Hashtable graphicsobjects = new Hashtable ();
			Hashtable imageformatobjects = new Hashtable ();

			XmlTextReader reader = new XmlTextReader (new StringReader (Script));

			while (reader.Read ())
			{
				switch (reader.NodeType)
				{
					case XmlNodeType.Element:

						switch (reader.Name.ToLower ())
						{
							#region FILES
							case "filecopy":
							{
								string pathsource = string.Empty;
								string pathdestination = string.Empty;

								while (reader.MoveToNextAttribute ())
								{
									switch (reader.Name.ToLower ())
									{
										case "pathsource":
										{
											pathsource = ReplaceTags (reader.Value, variables);
//											pathsource = reader.Value.Replace ("%%PATHSOURCE%%", PathSource).Replace ("%%HTMLROOT%%", SorentoLib.Services.Config.Get<string> (Enums.ConfigKey.core_pathhtmlroot));
											break;
										}

										case "pathdestination":
										{
											pathdestination = ReplaceTags (reader.Value, variables);
											break;
										}
									}
								}

								SNDK.IO.CopyFile (pathsource, pathdestination);

								SorentoLib.Services.Logging.LogDebug ("FileCopy : "+ pathsource +" to "+ pathdestination);
								break;
							}
							#endregion

							#region GRAPHICS

							#region NEW
							case "new":
							{
								string name = string.Empty;
								int width = 0;
								int height = 0;

								while (reader.MoveToNextAttribute ())
								{
									switch (reader.Name.ToLower ())
									{
										case "name":
										{
											name = reader.Value;
											break;
										}

										case "width":
										{
											if (SNDK.Helpers.IsValidAlphaNumeric (reader.Value))
											{
												width = int.Parse (reader.Value);
											}
											else
											{
												string[] split =  reader.Value.Split (".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
												if (split[1].ToLower() == "width")
												{
													width = ((Bitmap)graphicsobjects[split[0]]).Width;
												}
												else if (split[1].ToLower() == "height")
												{
													width = ((Bitmap)graphicsobjects[split[0]]).Height;
												}
											}
											break;
										}

										case "height":
										{
											if (SNDK.Helpers.IsValidAlphaNumeric (reader.Value))
											{
												height = int.Parse (reader.Value);
											}
											else
											{
												string[] split =  reader.Value.Split (".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
												if (split[1].ToLower() == "width")
												{
													height = ((Bitmap)graphicsobjects[split[0]]).Width;
												}
												else if (split[1].ToLower() == "height")
												{
													height = ((Bitmap)graphicsobjects[split[0]]).Height;
												}
											}
											break;
										}
									}
								}

								graphicsobjects[name] = new Bitmap (width, height);

								SorentoLib.Services.Logging.LogDebug ("GraphicsNew : "+ name +" object with dimensions "+ width +" x "+ height);
								break;
							}
							#endregion

							#region LOAD
							case "load":
							{
								string name = string.Empty;
								string path = string.Empty;

								while (reader.MoveToNextAttribute ())
								{
									switch (reader.Name.ToLower())
									{
										case "name":
										{
											name = reader.Value;
											break;
										}

										case "path":
										{
											path = ReplaceTags (reader.Value, variables);
											break;
										}
									}
								}


								SorentoLib.Services.Logging.LogDebug ("GraphicsLoad : "+ path +" into "+ name +" object.");

								graphicsobjects[name] = SNDK.Graphics.Load (path);


								imageformatobjects[name] = ((Bitmap)graphicsobjects[name]).RawFormat;

//								if (imageformatobjects[name].Equals(System.Drawing.Imaging.ImageFormat.Jpeg))
//								{
//									Console.WriteLine ("jpeg");
//								}
//
//								if (imageformatobjects[name].Equals(System.Drawing.Imaging.ImageFormat.Png))
//								{
//									Console.WriteLine ("png");
//								}


								break;
							}
							#endregion

							#region SAVE
							case "save":
							{
								string source = string.Empty;
								string path = string.Empty;
								string codec = string.Empty;
								string altcodec = string.Empty;
								int compression = 100;
								long quality = 80;

								while (reader.MoveToNextAttribute())
								{
									switch (reader.Name.ToLower())
									{
										case "source":
										{
											source = reader.Value;
											break;
										}

										case "path":
										{
											path = ReplaceTags (reader.Value, variables);
											break;
										}

										case "codec":
										{
											codec = reader.Value;
											break;
										}

										case "compression":
										{
											compression = int.Parse (reader.Value);
											break;
										}

										case "quality":
										{
											quality = long.Parse (reader.Value);
											break;
										}
									}
								}



								if (codec == string.Empty)
								{
									try
									{
										if (imageformatobjects[source].Equals(System.Drawing.Imaging.ImageFormat.Jpeg))
										{
											codec = "jpeg";
										}
										else if (imageformatobjects[source].Equals(System.Drawing.Imaging.ImageFormat.Png))
										{
											codec = "png";
										}
										else if (imageformatobjects[source].Equals(System.Drawing.Imaging.ImageFormat.Gif))
										{
											codec = "gif";
										}
									}
									catch
									{}
								}

								switch (codec)
								{
									#region GIF
									case "gif":
									{
										SNDK.Graphics.SaveGif (path, (Bitmap)graphicsobjects[source]);
										break;
									}
									#endregion

									#region PNG
									case "png":
									{
										SNDK.Graphics.SavePng (path, (Bitmap)graphicsobjects[source], compression);
										break;
									}
									#endregion

									#region JPEG
									case "jpeg":
									{
										SNDK.Graphics.SaveJpeg(path, (Bitmap)graphicsobjects[source],  quality);
										break;
									}
									#endregion
								}

								SorentoLib.Services.Logging.LogDebug ("Saving "+ codec +": "+ source +" object to "+ path);
								break;
							}
							#endregion

							#region SCALE
							case "scale":
							{
								string source = string.Empty;
								int width = 0;
								int height = 0;
								bool keepaspect = false;
								bool exact = false;

								while (reader.MoveToNextAttribute ())
								{
									switch (reader.Name.ToLower ())
									{
										case "source":
										{
											source = reader.Value;
											break;
										}

										case "width":
										{
											if (SNDK.Helpers.IsValidAlphaNumeric (reader.Value))
											{
												width = int.Parse (reader.Value);
											}
											else
											{
												string[] split = reader.Value.Split (".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
												if (split[1].ToLower () == "width")
												{
													width = ((Bitmap)graphicsobjects[split[0]]).Width;
												}
												else if (split[1].ToLower () == "height")
												{
													width = ((Bitmap)graphicsobjects[split[0]]).Height;
												}
											}
											break;
										}

										case "height":
										{
											if (SNDK.Helpers.IsValidAlphaNumeric (reader.Value))
											{
												height = int.Parse (reader.Value);
											}
											else
											{
												string[] split = reader.Value.Split (".".ToCharArray());
												if (split[1].ToLower () == "width")
												{
													height = ((Bitmap)graphicsobjects[split[0]]).Width;
												}
												else if (split[1].ToLower () == "height")
												{
													height = ((Bitmap)graphicsobjects[split[0]]).Height;
												}
											}
											break;
										}

										case "keepaspect":
										{
											if (reader.Value.ToLower () == "true")
											{
												keepaspect = true;
											}
											break;
										}

										case "exact":
										{
											if (reader.Value.ToLower () == "true")
											{
												exact = true;
											}
											break;
										}
									}
								}


								graphicsobjects[source] = SNDK.Graphics.Scale ((Bitmap)graphicsobjects[source], width, height, keepaspect, exact);

								SorentoLib.Services.Logging.LogDebug ("Scaling: "+ source +" object to dimensions "+ width +" x "+ height +"  keepaspect "+ keepaspect +"  exact "+ exact);
								break;
							}
							#endregion

							#region PASTE
							case "paste":
							{
								string source = string.Empty;
								string destination = string.Empty;
								int x = 0;
								int y = 0;
								int areax = 0;
								int areay = 0;
								int areawidth = 0;
								int areaheight = 0;

								while (reader.MoveToNextAttribute ())
								{
									switch (reader.Name.ToLower ())
									{
										case "source":
										{
											source = reader.Value;
											break;
										}

										case "destination":
										{
											destination = reader.Value;
											break;
										}

										case "x":
										{
											if (SNDK.Helpers.IsValidAlphaNumeric (reader.Value))
											{
												x = int.Parse (reader.Value);
											}
											else
											{
												string[] split = reader.Value.Split (".".ToCharArray (), StringSplitOptions.RemoveEmptyEntries);
												if (split[1].ToLower () == "width")
												{
													x = ((Bitmap)graphicsobjects[split[0]]).Width;
												}
												else if (split[1].ToLower () == "height")
												{
													x = ((Bitmap)graphicsobjects[split[0]]).Height;
												}
											}
											break;
										}

										case "y":
										{
											if (SNDK.Helpers.IsValidAlphaNumeric (reader.Value))
											{
												y = int.Parse (reader.Value);
											}
											else
											{
												string[] split = reader.Value.Split (".".ToCharArray (), StringSplitOptions.RemoveEmptyEntries);
												if (split[1].ToLower () == "width")
												{
													y = ((Bitmap)graphicsobjects[split[0]]).Width;
												}
												else if (split[1].ToLower () == "height")
												{
													y = ((Bitmap)graphicsobjects[split[0]]).Height;
												}
											}
											break;
										}

										case "areax":
										{
											if (SNDK.Helpers.IsValidAlphaNumeric (reader.Value))
											{
												areax = int.Parse (reader.Value);
											}
											else
											{
												string[] split = reader.Value.Split (".".ToCharArray(), StringSplitOptions.None);
												if (split[1].ToLower () == "width")
												{
													areax = ((Bitmap)graphicsobjects[split[0]]).Width;
												}
												else if (split[1].ToLower () == "height")
												{
													areax = ((Bitmap)graphicsobjects[split[0]]).Height;
												}
											}
											break;
										}

										case "areay":
										{
											if (SNDK.Helpers.IsValidAlphaNumeric (reader.Value))
											{
												areay = int.Parse (reader.Value);
											}
											else
											{
												string[] split = reader.Value.Split (".".ToCharArray (), StringSplitOptions.RemoveEmptyEntries);
												if (split[1].ToLower () == "width")
												{
													areay = ((Bitmap)graphicsobjects[split[0]]).Width;
												}
												else if (split[1].ToLower () == "height")
												{
													areay = ((Bitmap)graphicsobjects[split[0]]).Height;
												}
											}
											break;
										}

										case "areawidth":
										{
											if (SNDK.Helpers.IsValidAlphaNumeric (reader.Value))
											{
												areawidth = int.Parse (reader.Value);
											}
											else
											{
												string[] split = reader.Value.Split (".".ToCharArray (), StringSplitOptions.RemoveEmptyEntries);
												if (split[1].ToLower () == "width")
												{
													areawidth = ((Bitmap)graphicsobjects[split[0]]).Width;
												}
												else if (split[1].ToLower () == "height")
												{
													areawidth = ((Bitmap)graphicsobjects[split[0]]).Height;
												}
											}
											break;
										}

										case "areaheight":
										{
											if (SNDK.Helpers.IsValidAlphaNumeric (reader.Value))
											{
												areaheight = int.Parse (reader.Value);
											}
											else
											{
												string[] split = reader.Value.Split (".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
												if (split[1].ToLower() == "width")
												{
													areaheight = ((Bitmap)graphicsobjects[split[0]]).Width;
												}
												else if (split[1].ToLower () == "height")
												{
													areaheight = ((Bitmap)graphicsobjects[split[0]]).Height;
												}
											}
											break;
										}
									}
								}

								graphicsobjects[destination] = SNDK.Graphics.Paste ((Bitmap)graphicsobjects[destination], (Bitmap)graphicsobjects[source], x, y, new Rectangle (areax, areay, areawidth, areaheight));

								SorentoLib.Services.Logging.LogDebug ("Pasting: "+ source +" object onto "+ destination +" at "+ x +" x "+ y +" with dimensions "+ areax +" x "+ areay +" x "+ areawidth +" x "+ areaheight);
								break;
							}
							#endregion
						
							#region ROTATE
							case "rotate":
							{
								string source = string.Empty;
								float angle = 0;

								while (reader.MoveToNextAttribute ())
								{
									switch (reader.Name.ToLower ())
									{
										case "source":
										{
											source = reader.Value;
											break;
										}

										case "angle":
										{
											angle = float.Parse (reader.Value);

//											attangle = reader.Value;
//											Random random = new Random();
//											attangle = random.Next(-20, 20).ToString();

											break;
										}
									}
								}

								graphicsobjects[source] = SNDK.Graphics.Rotate ((Bitmap)graphicsobjects[source], angle);

								SorentoLib.Services.Logging.LogDebug ("Rotating: "+ source +" object at "+ angle);
								break;
							}
							#endregion

							#region TEXT
							case "text":
							{
								string source = string.Empty;
								string text = string.Empty;
								string fontpath = string.Empty;
								float size = 0;
								int alpha = 0;
								int r = 0;
								int g = 0;
								int b = 0;
								int x = 0;
								int y = 0;
								string align = string.Empty;

								while (reader.MoveToNextAttribute ())
								{
									switch (reader.Name.ToLower())
									{
										case "source":
										{
											source = reader.Value;
											break;
										}

										case "text":
										{
											text = reader.Value;
											break;
										}

										case "fontpath":
										{
											fontpath = ReplaceTags (reader.Value, variables);
											break;
										}

										case "size":
										{
											size = float.Parse (reader.Value);
											break;
										}

										case "color":
										{
											string[] split = reader.Value.Split (",".ToCharArray (), StringSplitOptions.RemoveEmptyEntries);

											alpha = int.Parse (split[0]);
											r = int.Parse (split[1]);
											g = int.Parse (split[2]);
											b = int.Parse (split[3]);
											break;
										}

										case "x":
										{
											x = int.Parse (reader.Value);
											break;
										}

										case "y":
										{
											y = int.Parse (reader.Value);
											break;
										}

										case "align":
										{
											align = reader.Value;
											break;
										}
									}
								}

								graphicsobjects[source] = SNDK.Graphics.Text ((Bitmap)graphicsobjects[source], text, fontpath, size, Color.FromArgb (alpha, r, g, b), x, y, align);

								SorentoLib.Services.Logging.LogDebug ("Writing: "+ text +" on "+ source +" object using "+ fontpath +" size "+ size +" at "+ x +"x"+ y +" aligned "+ align);
								break;
							}
							#endregion

							#endregion
						}
						break;
				}
			}

			reader.Close ();

			return result;
		}
	}
}

