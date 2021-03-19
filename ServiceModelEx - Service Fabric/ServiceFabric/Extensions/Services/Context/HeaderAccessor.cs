// © 2017 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using ServiceModelEx.ServiceFabric.Services.Remoting;

namespace ServiceModelEx.ServiceFabric.Extensions.Services
{
   public interface IHeaderAccessor
   {
      /// <summary>
      /// Add a header to the message before the message is sent.
      /// </summary>
      /// <typeparam name="T">A header type expressed as a DataContract</typeparam>
      /// <param name="header">the instance of a header to add</param>
      void AddHeader<T>(T header) where T : class;
      /// <summary>
      /// Get the header of type T from the message.
      /// </summary>
      /// <typeparam name="T">A header type expressed as a DataContract<</typeparam>
      T GetHeader<T>() where T : class;
      /// <summary>
      /// Copy the header of type T from a service's header cache to the message before the message is sent.
      /// </summary>
      /// <typeparam name="T">A header type expressed as a DataContract<</typeparam>
      void CopyHeaderTo<T>() where T : class;
      /// <summary>
      /// Copy the header of type T from the message to a service's header cache after the response has been received.
      /// </summary>
      /// <typeparam name="T">A header type expressed as a DataContract<</typeparam>
      void CopyHeaderFrom<T>() where T : class;
   }

   public class HeaderAccessor : IHeaderAccessor
   {
      internal ServiceRemotingMessageHeaders Headers 
      {get;set;}

      public HeaderAccessor() : this(new ServiceRemotingMessageHeaders())
      {}
      internal HeaderAccessor(ServiceRemotingMessageHeaders headers)
      {
         Headers = headers;
      }

      public void AddHeader<T>(T header) where T : class
      {
         Header<T>.Add(header,Headers);
      }
      public T GetHeader<T>() where T : class
      {
         return Header<T>.Get(Headers);
      }
      public void CopyHeaderTo<T>() where T : class
      {
         Header<T>.CopyHeaderTo(Headers);
      }
      public void CopyHeaderFrom<T>() where T : class
      {
         Header<T>.CopyHeaderFrom(Headers);
      }
   }
}
