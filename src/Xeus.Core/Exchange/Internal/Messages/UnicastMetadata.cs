using System;
using System.IO;
using Omnix.Base;
using Omnix.Cryptography;
using Omnix.Serialization;
using Xeus.Messages;

namespace Xeus.Core.Exchange.Internal
{
    sealed partial class UnicastMetadata
    {
        public static UnicastMetadata Create(string type, OmniSignature signature, Timestamp creationTime, Clue clue, OmniDigitalSignature digitalSignature)
        {
            var hub = new Hub();

            try
            {
                var target = new UnicastMetadata(type, signature, creationTime, clue, null);
                target.Export(hub.Writer, BufferPool.Shared);
                hub.Writer.Complete();

                var certificate = OmniCertificate.Create(digitalSignature, hub.Reader.GetSequence());
                hub.Reader.Complete();

                return new UnicastMetadata(type, signature, creationTime, clue, certificate);
            }
            finally
            {
                hub.Reset();
            }
        }

        public bool VerifyCertificate()
        {
            var hub = new Hub();

            try
            {
                var target = new UnicastMetadata(this.Type, this.Signature, this.CreationTime, this.Clue, null);
                target.Export(hub.Writer, BufferPool.Shared);
                hub.Writer.Complete();

                var result = this.Certificate.Verify(hub.Reader.GetSequence());
                hub.Reader.Complete();

                return result;
            }
            finally
            {
                hub.Reset();
            }
        }
    }
}
