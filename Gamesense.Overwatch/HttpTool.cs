using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Gamesense.Overwatch
{
	public class HttpTool
	{
		HttpClient _client;

		public JavaScriptSerializer Serializer;

		public HttpTool()
		{
			Serializer = new JavaScriptSerializer();
			Serializer.RegisterConverters(new JavaScriptConverter[] { new DataContractJavaScriptConverter(true) });

			string cpPath = Path.Combine(Environment.GetEnvironmentVariable("PROGRAMDATA"), "SteelSeries", "SteelSeries Engine 3", "coreProps.json");
			var cprops = Serializer.Deserialize<CoreProps>(File.ReadAllText(cpPath));
			Util.WriteLog($"Found SSE3 endpoint at {cprops.BaseAddress}");

			_client = new HttpClient() { BaseAddress = new Uri(cprops.BaseAddress) };
		}

		public async Task<HttpResponseMessage> GetAsync(string path)
		{
			return await _client.GetAsync(path);
		}

		public async Task<string> PostAsync<T>(string path, T data)
		{
			var request = new HttpRequestMessage(HttpMethod.Post, _client.BaseAddress + path);
			using (var content = new StringContent(Serializer.Serialize(data), System.Text.Encoding.UTF8, "application/json"))
			{
				Util.WriteLog($"Posting content: {content.ReadAsStringAsync().Result}");
				request.Content = content;

				await _client.SendAsync(request).ConfigureAwait(false);
				var response = await _client.SendAsync(request).ConfigureAwait(false);
				//response.EnsureSuccessStatusCode();
				return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			}
		}

		public class CoreProps
		{
			public string Address { get; set; }
			public string Encrypted_address { get; set; }
			public string BaseAddress { get { return "http://" + Address; } }
		}

		public class DataContractJavaScriptConverter : JavaScriptConverter
		{
			private static readonly List<Type> _supportedTypes = new List<Type>();

			static DataContractJavaScriptConverter()
			{
				foreach (Type type in Assembly.GetExecutingAssembly().DefinedTypes)
				{
					if (Attribute.IsDefined(type, typeof(DataContractAttribute)))
					{
						_supportedTypes.Add(type);
					}
				}
			}

			private bool ConvertEnumToString = false;

			public DataContractJavaScriptConverter() : this(false)
			{
			}

			public DataContractJavaScriptConverter(bool convertEnumToString)
			{
				ConvertEnumToString = convertEnumToString;
			}

			public override IEnumerable<Type> SupportedTypes
			{
				get { return _supportedTypes; }
			}

			public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
			{
				if (Attribute.IsDefined(type, typeof(DataContractAttribute)))
				{
					try
					{
						object instance = Activator.CreateInstance(type);

						IEnumerable<MemberInfo> members = ((IEnumerable<MemberInfo>)type.GetFields())
							.Concat(type.GetProperties().Where(property => property.CanWrite && property.GetIndexParameters().Length == 0))
							.Where((member) => Attribute.IsDefined(member, typeof(DataMemberAttribute)));
						foreach (MemberInfo member in members)
						{
							DataMemberAttribute attribute = (DataMemberAttribute)Attribute.GetCustomAttribute(member, typeof(DataMemberAttribute));
							object value;
							if (dictionary.TryGetValue(attribute.Name, out value) == false)
							{
								if (attribute.IsRequired)
								{
									throw new SerializationException(String.Format("Required DataMember with name {0} not found", attribute.Name));
								}
								continue;
							}
							if (member.MemberType == MemberTypes.Field)
							{
								FieldInfo field = (FieldInfo)member;
								object fieldValue;
								if (ConvertEnumToString && field.FieldType.IsEnum)
								{
									fieldValue = Enum.Parse(field.FieldType, value.ToString());
								}
								else
								{
									fieldValue = serializer.ConvertToType(value, field.FieldType);
								}
								field.SetValue(instance, fieldValue);
							}
							else if (member.MemberType == MemberTypes.Property)
							{
								PropertyInfo property = (PropertyInfo)member;
								object propertyValue;
								if (ConvertEnumToString && property.PropertyType.IsEnum)
								{
									propertyValue = Enum.Parse(property.PropertyType, value.ToString());
								}
								else
								{
									propertyValue = serializer.ConvertToType(value, property.PropertyType);
								}
								property.SetValue(instance, propertyValue);
							}
						}
						return instance;
					}
					catch (Exception)
					{
						return null;
					}
				}
				return null;
			}

			public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				if (obj != null && Attribute.IsDefined(obj.GetType(), typeof(DataContractAttribute)))
				{
					Type type = obj.GetType();
					IEnumerable<MemberInfo> members = ((IEnumerable<MemberInfo>)type.GetFields())
						.Concat(type.GetProperties().Where(property => property.CanRead && property.GetIndexParameters().Length == 0))
						.Where((member) => Attribute.IsDefined(member, typeof(DataMemberAttribute)));
					foreach (MemberInfo member in members)
					{
						DataMemberAttribute attribute = (DataMemberAttribute)Attribute.GetCustomAttribute(member, typeof(DataMemberAttribute));
						object value;
						if (member.MemberType == MemberTypes.Field)
						{
							FieldInfo field = (FieldInfo)member;
							if (ConvertEnumToString && field.FieldType.IsEnum)
							{
								value = field.GetValue(obj).ToString();
							}
							else
							{
								value = field.GetValue(obj);
							}
						}
						else if (member.MemberType == MemberTypes.Property)
						{
							PropertyInfo property = (PropertyInfo)member;
							if (ConvertEnumToString && property.PropertyType.IsEnum)
							{
								value = property.GetValue(obj).ToString();
							}
							else
							{
								value = property.GetValue(obj);
							}
						}
						else
						{
							continue;
						}
						if (dictionary.ContainsKey(attribute.Name))
						{
							throw new SerializationException(String.Format("More than one DataMember found with name {0}", attribute.Name));
						}
						dictionary[attribute.Name] = value;
					}
				}
				return dictionary;
			}
		}
	}
}
