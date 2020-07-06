using System;
using System.Collections.Generic;
using System.Linq;
using Common.Http;
using Common.SignalR.EventBus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;

namespace Common.SignalR
{
    public static class SignalRExtensions
    {
        public static HttpContext TryGetHttpContext(this Hub hub)
        {
            return hub?.Context?.GetHttpContext();
        }

        public static string TryGetScopeId(this Hub hub)
        {
            return hub?.TryGetHttpContext().TryGetQueryParameterValue(HubConst.Args_ScopeId, HubConst.DefaultScopeId);
        }

        public static string TryGetClientId(this Hub hub)
        {
            return hub?.TryGetHttpContext().TryGetQueryParameterValue(HubConst.Args_ClientId, string.Empty);
        }

        public static string TryGetConnectionId(this Hub hub)
        {
            //return hub?.TryGetHttpContext().TryGetQueryParameterValue(HubConst.Args_Id, string.Empty);
            return hub.Context.ConnectionId;
        }

        public static T AutoFixScopeId<T>(this T args, Hub hub) where T : IScopeKey
        {
            if (string.IsNullOrWhiteSpace(args.ScopeId))
            {
                args.ScopeId = hub.TryGetScopeId();
            }

            return args;
        }

        public static IHubClients<IClientProxy> TryGetHubClients(this ISignalREvent theEvent)
        {
            if (theEvent is IHubEvent hubEvent)
            {
                if (hubEvent.RaiseHub != null)
                {
                    return hubEvent.RaiseHub.Clients;
                }
            }

            if (theEvent is IHubContextEvent hubContextEvent)
            {
                if (hubContextEvent.Context != null)
                {
                    return hubContextEvent.Context.Clients;
                }
            }
            throw new ArgumentException("event must have not null RaiseHub or Context");


            //switch (theEvent)
            //{
            //    //IHubCrossEvent : ISignalREvent, IHubEvent, IHubContextEvent
            //    case IHubEvent hubEvent:
            //        return hubEvent.RaiseHub.Clients;
            //    case IHubContextEvent hubContextEvent:
            //        return hubContextEvent.Context.Clients;
            //    default:
            //        throw new ArgumentException(nameof(theEvent));
            //}
        }

        public static string GetCurrentScope(this ISignalREvent theEvent)
        {
            //if (!string.IsNullOrWhiteSpace(theEvent.ScopeId))
            //{
            //    return theEvent.ScopeId;
            //}
            var hubClients = theEvent.TryGetHubClients();
            return null;
        }
        public static IClientProxy ScopeAll(this IHubClients<IClientProxy> hubClients, string scope)
        {
            return null;
        }

        public static IClientProxy ScopeGroup(this IHubClients<IClientProxy> hubClients, string scope, string group)
        {
            return null;
        }

        public static IClientProxy ScopeClients(this IHubClients<IClientProxy> hubClients, string scope, IEnumerable<string> clientIds)
        {
            return null;
        }

        public static IClientProxy GetClientProxy(this IHubClients<IClientProxy> hubClients, string scope, IEnumerable<string> clientIds)
        {
            var connectionIds = GetConnectionIds(scope, clientIds).ToList();
            return hubClients.Clients(connectionIds);
        }

        public static IList<string> GetConnectionIds(string scope, IEnumerable<string> clientIds)
        {
            //todo
            return new List<string>();
        }
    }
}
