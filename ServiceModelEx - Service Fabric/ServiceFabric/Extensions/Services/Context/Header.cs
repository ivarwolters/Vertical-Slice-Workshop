// © 2017 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;

using ServiceModelEx.Fabric;
using ServiceModelEx.ServiceFabric.Services.Remoting;

namespace ServiceModelEx.ServiceFabric.Extensions.Services
{
   public static class Header<T> where T : class
   {
      static string TypeName
      {
         get
         {
            return typeof(T).Name;
         }
      }
      static string TypeNamespace
      {
         get
         {
            return typeof(T).FullName;
         }
      }

      static Header()
      {
         // Verify [DataContract] or [Serializable] on T
         Debug.Assert(IsDataContract(typeof(T)) || typeof(T).IsSerializable);
      }
      static bool IsDataContract(Type type)
      {
         object[] attributes = type.GetCustomAttributes(typeof(DataContractAttribute),false);
         return attributes.Length == 1;
      }
      static bool IsHeaderNotExistsException(Exception exception)
      {
         return (exception is MessageHeaderException && exception.Message == HeaderNotFoundMessage);
      }
      static string HeaderNotFoundMessage
      {
         get
         {
            return "There is not a header with name " + TypeName + " and namespace " + TypeNamespace + " in the message.";
         }
      }
      static string ValidOperationContextRequiredMessage
      {
         get
         {
            return "This action is only valid within the context of a service operation. A valid OperationContext is required.";
         }
      }

      internal static ServiceRemotingMessageHeaders Get(MessageHeaders headers)
      {
         Debug.Assert(headers != null);

         try
         {
            if(headers.FindHeader(typeof(ServiceRemotingMessageHeaders).Name,typeof(ServiceRemotingMessageHeaders).FullName) < 0)
            {
               return new ServiceRemotingMessageHeaders();
            }
            else
            {
               return headers.GetHeader<ServiceRemotingMessageHeaders>(typeof(ServiceRemotingMessageHeaders).Name,typeof(ServiceRemotingMessageHeaders).FullName);
            }
         }
         catch(Exception exception)
         {
            Debug.Assert(IsHeaderNotExistsException(exception));
            return null;
         }
      }
      internal static void Replace(ServiceRemotingMessageHeaders value,MessageHeaders headers)
      {
         Debug.Assert(value != default(T));
         Debug.Assert(headers != null);

         int index = headers.FindHeader(typeof(ServiceRemotingMessageHeaders).Name,typeof(ServiceRemotingMessageHeaders).FullName);
         if(index >= 0)
         {
            headers.RemoveAt(index);
         }
         MessageHeader<ServiceRemotingMessageHeaders> genericHeader = new MessageHeader<ServiceRemotingMessageHeaders>(value);
         headers.Add(genericHeader.GetUntypedHeader(typeof(ServiceRemotingMessageHeaders).Name,typeof(ServiceRemotingMessageHeaders).FullName));
      }

      public static T Get(ServiceRemotingMessageHeaders headers)
      {
         T header = default(T);
         byte[] bytes = null;
         if(headers.TryGetHeaderValue(typeof(T).Name,out bytes) == true)
         {
            if(bytes.Length > 0)
            {
               MemoryStream reader = new MemoryStream(bytes);
               DataContractSerializer serializer = new DataContractSerializer(typeof(T));
               header = serializer.ReadObject(reader) as T;
               reader.Close();
            }
         }
         return header;
      }
      public static T GetIncoming()
      {
         OperationContext context = OperationContext.Current;
         Debug.Assert(context != null);
         Debug.Assert(context.IncomingMessageHeaders != null);
         if(context == null || context.IncomingMessageHeaders == null)
         {
            return default(T);
         }
         return Get(Get(context.IncomingMessageHeaders));
      }
      public static T GetOutgoing()
      {
         OperationContext context = OperationContext.Current;
         Debug.Assert(context != null);
         Debug.Assert(context.OutgoingMessageHeaders != null);
         if(context == null || context.OutgoingMessageHeaders == null)
         {
            Debug.Assert(false);
            return default(T);
         }

         return Get(Get(context.OutgoingMessageHeaders));
      }

      public static void Add(T value,ServiceRemotingMessageHeaders headers)
      {
         Debug.Assert(value != default(T));
         Debug.Assert(headers != null);

         byte[] bytes = null;
         if(headers.TryGetHeaderValue(TypeName,out bytes))
         {
            throw new FabricElementAlreadyExistsException("A header with name " + TypeName + " and namespace " + TypeNamespace + " already exists in the message.");
         }

         MemoryStream writer = new MemoryStream();
         DataContractSerializer serializer = new DataContractSerializer(typeof(T));
         serializer.WriteObject(writer,value);
         writer.Close();

         bytes = writer.ToArray();
         headers.AddHeader(TypeName,bytes);
      }
      public static void AddIncoming(T value)
      {
         OperationContext context = OperationContext.Current;
         Debug.Assert(context != null);
         Debug.Assert(context.OutgoingMessageHeaders != null);
         if(context != null && context.IncomingMessageHeaders != null)
         {
            ServiceRemotingMessageHeaders headers = Get(context.IncomingMessageHeaders);
            Add(value,headers);
            Replace(headers,context.IncomingMessageHeaders);
         }
      }
      public static void AddOutgoing(T value)
      {
         OperationContext context = OperationContext.Current;
         Debug.Assert(context != null);
         Debug.Assert(context.OutgoingMessageHeaders != null);
         if(context != null && context.OutgoingMessageHeaders != null)
         {
            ServiceRemotingMessageHeaders headers = Get(context.OutgoingMessageHeaders);
            Add(value,headers);
            Replace(headers,context.OutgoingMessageHeaders);
         }
      }

      public static void Replace(T value,ServiceRemotingMessageHeaders headers)
      {
         Debug.Assert(value != default(T));
         Debug.Assert(headers != null);

         byte[] bytes = null;
         if(headers.TryGetHeaderValue(TypeName,out bytes) == false)
         {
            throw new FabricElementNotFoundException(HeaderNotFoundMessage);
         }
         if(headers.RemoveHeader(TypeName))
         {
            Add(value,headers);
         }
      }
      public static void ReplaceIncoming(T value)
      {
         OperationContext context = OperationContext.Current;
         Debug.Assert(context != null);
         Debug.Assert(context.IncomingMessageHeaders != null);
         if(context == null || context.IncomingMessageHeaders == null)
         {
            throw new InvalidOperationException(ValidOperationContextRequiredMessage);
         }

         ServiceRemotingMessageHeaders headers = Get(context.IncomingMessageHeaders);
         Replace(value,headers);
         Replace(headers,context.IncomingMessageHeaders);
      }
      public static void ReplaceOutgoing(T value)
      {
         OperationContext context = OperationContext.Current;
         Debug.Assert(context != null);
         Debug.Assert(context.OutgoingMessageHeaders != null);
         if(context == null || context.OutgoingMessageHeaders == null)
         {
            throw new InvalidOperationException(ValidOperationContextRequiredMessage);
         }

         ServiceRemotingMessageHeaders headers = Get(context.OutgoingMessageHeaders);
         Replace(value,headers);
         Replace(headers,context.OutgoingMessageHeaders);
      }

      public static void CopyHeader(ServiceRemotingMessageHeaders source,ServiceRemotingMessageHeaders destination)
      {
         Debug.Assert(source != null);
         Debug.Assert(destination != null);

         T sourceHeader = Get(source);
         T destinationHeader = Get(destination);

         if(sourceHeader != null && destination != null)
         {
            if(destinationHeader == null)
            {
               Add(sourceHeader,destination);
            }
            else
            {
               Replace(sourceHeader,destination);
            }
         }
      }
      public static void CopyHeaderTo(ServiceRemotingMessageHeaders destination)
      {
         OperationContext context = OperationContext.Current;
         Debug.Assert(context != null);
         Debug.Assert(context.IncomingMessageHeaders != null);
         if(context == null || context.IncomingMessageHeaders == null)
         {
            throw new InvalidOperationException(ValidOperationContextRequiredMessage);
         }

         CopyHeader(Get(context.IncomingMessageHeaders),destination);
      }
      public static void CopyHeaderFrom(ServiceRemotingMessageHeaders source)
      {
         OperationContext context = OperationContext.Current;
         Debug.Assert(context != null);
         Debug.Assert(context.IncomingMessageHeaders != null);
         if(context == null || context.IncomingMessageHeaders == null)
         {
            throw new InvalidOperationException(ValidOperationContextRequiredMessage);
         }

         ServiceRemotingMessageHeaders headers = Get(context.IncomingMessageHeaders);
         CopyHeader(source,headers);
         Replace(headers,context.IncomingMessageHeaders);
      }
   }

}
