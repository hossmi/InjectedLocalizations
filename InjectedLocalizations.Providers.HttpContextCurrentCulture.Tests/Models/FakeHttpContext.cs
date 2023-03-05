using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Http.Features;

namespace InjectedLocalizations.Models
{
    public class FakeHttpContext : HttpContext
    {
        FakeFeatureCollection features;

        public FakeHttpContext(FakeFeatureCollection features = null)
        {
            this.features = features ?? new FakeFeatureCollection();
        }

        public override ConnectionInfo Connection { get; }
        public override IFeatureCollection Features => this.features;
        public override IDictionary<object, object> Items { get; set; }
        public override HttpRequest Request { get; }
        public override CancellationToken RequestAborted { get; set; }
        public override IServiceProvider RequestServices { get; set; }
        public override HttpResponse Response { get; }
        public override ISession Session { get; set; }
        public override string TraceIdentifier { get; set; }
        public override ClaimsPrincipal User { get; set; }
        public override WebSocketManager WebSockets { get; }
        public override AuthenticationManager Authentication { get; }

        public override void Abort()
        {
            throw new NotImplementedException();
        }

        public void SetFeature<TFeature>(TFeature feature) => this.features.Set(feature);
    }
}
