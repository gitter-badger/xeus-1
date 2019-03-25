using Omnix.Base;
using Omnix.Base.Helpers;
using Omnix.Cryptography;
using Omnix.Network;
using Omnix.Serialization;
using Omnix.Serialization.RocketPack;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xeus.Messages;
using Xeus.Messages.Options;
using Xeus.Messages.Reports;

namespace Xeus.Core.Exchange.Internal
{
    internal enum ProtocolVersion : byte
    {
        Version1 = 1,
    }

    internal sealed partial class BroadcastClue : RocketPackMessageBase<BroadcastClue>
    {
        static BroadcastClue()
        {
            BroadcastClue.Formatter = new CustomFormatter();
        }

        public static readonly int MaxTypeLength = 256;

        public BroadcastClue(string type, Timestamp creationTime, Clue clue, OmniCertificate certificate)
        {
            if (type is null) throw new ArgumentNullException("type");
            if (type.Length > 256) throw new ArgumentOutOfRangeException("type");
            if (clue is null) throw new ArgumentNullException("clue");
            if (certificate is null) throw new ArgumentNullException("certificate");

            this.Type = type;
            this.CreationTime = creationTime;
            this.Clue = clue;
            this.Certificate = certificate;

            {
                var hashCode = new HashCode();
                if (this.Type != default) hashCode.Add(this.Type.GetHashCode());
                if (this.CreationTime != default) hashCode.Add(this.CreationTime.GetHashCode());
                if (this.Clue != default) hashCode.Add(this.Clue.GetHashCode());
                if (this.Certificate != default) hashCode.Add(this.Certificate.GetHashCode());
                _hashCode = hashCode.ToHashCode();
            }
        }

        public string Type { get; }
        public Timestamp CreationTime { get; }
        public Clue Clue { get; }
        public OmniCertificate Certificate { get; }

        public override bool Equals(BroadcastClue target)
        {
            if ((object)target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;
            if (this.Type != target.Type) return false;
            if (this.CreationTime != target.CreationTime) return false;
            if (this.Clue != target.Clue) return false;
            if (this.Certificate != target.Certificate) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        private sealed class CustomFormatter : IRocketPackFormatter<BroadcastClue>
        {
            public void Serialize(RocketPackWriter w, BroadcastClue value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Write property count
                {
                    uint propertyCount = 0;
                    if (value.Type != default) propertyCount++;
                    if (value.CreationTime != default) propertyCount++;
                    if (value.Clue != default) propertyCount++;
                    if (value.Certificate != default) propertyCount++;
                    w.Write(propertyCount);
                }

                // Type
                if (value.Type != default)
                {
                    w.Write((uint)0);
                    w.Write(value.Type);
                }
                // CreationTime
                if (value.CreationTime != default)
                {
                    w.Write((uint)1);
                    w.Write(value.CreationTime);
                }
                // Clue
                if (value.Clue != default)
                {
                    w.Write((uint)2);
                    Clue.Formatter.Serialize(w, value.Clue, rank + 1);
                }
                // Certificate
                if (value.Certificate != default)
                {
                    w.Write((uint)3);
                    OmniCertificate.Formatter.Serialize(w, value.Certificate, rank + 1);
                }
            }

            public BroadcastClue Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Read property count
                uint propertyCount = r.GetUInt32();

                string p_type = default;
                Timestamp p_creationTime = default;
                Clue p_clue = default;
                OmniCertificate p_certificate = default;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0: // Type
                            {
                                p_type = r.GetString(256);
                                break;
                            }
                        case 1: // CreationTime
                            {
                                p_creationTime = r.GetTimestamp();
                                break;
                            }
                        case 2: // Clue
                            {
                                p_clue = Clue.Formatter.Deserialize(r, rank + 1);
                                break;
                            }
                        case 3: // Certificate
                            {
                                p_certificate = OmniCertificate.Formatter.Deserialize(r, rank + 1);
                                break;
                            }
                    }
                }

                return new BroadcastClue(p_type, p_creationTime, p_clue, p_certificate);
            }
        }
    }

    internal sealed partial class UnicastClue : RocketPackMessageBase<UnicastClue>
    {
        static UnicastClue()
        {
            UnicastClue.Formatter = new CustomFormatter();
        }

        public static readonly int MaxTypeLength = 256;

        public UnicastClue(string type, OmniSignature signature, Timestamp creationTime, Clue clue, OmniCertificate certificate)
        {
            if (type is null) throw new ArgumentNullException("type");
            if (type.Length > 256) throw new ArgumentOutOfRangeException("type");
            if (signature is null) throw new ArgumentNullException("signature");
            if (clue is null) throw new ArgumentNullException("clue");
            if (certificate is null) throw new ArgumentNullException("certificate");

            this.Type = type;
            this.Signature = signature;
            this.CreationTime = creationTime;
            this.Clue = clue;
            this.Certificate = certificate;

            {
                var hashCode = new HashCode();
                if (this.Type != default) hashCode.Add(this.Type.GetHashCode());
                if (this.Signature != default) hashCode.Add(this.Signature.GetHashCode());
                if (this.CreationTime != default) hashCode.Add(this.CreationTime.GetHashCode());
                if (this.Clue != default) hashCode.Add(this.Clue.GetHashCode());
                if (this.Certificate != default) hashCode.Add(this.Certificate.GetHashCode());
                _hashCode = hashCode.ToHashCode();
            }
        }

        public string Type { get; }
        public OmniSignature Signature { get; }
        public Timestamp CreationTime { get; }
        public Clue Clue { get; }
        public OmniCertificate Certificate { get; }

        public override bool Equals(UnicastClue target)
        {
            if ((object)target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;
            if (this.Type != target.Type) return false;
            if (this.Signature != target.Signature) return false;
            if (this.CreationTime != target.CreationTime) return false;
            if (this.Clue != target.Clue) return false;
            if (this.Certificate != target.Certificate) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        private sealed class CustomFormatter : IRocketPackFormatter<UnicastClue>
        {
            public void Serialize(RocketPackWriter w, UnicastClue value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Write property count
                {
                    uint propertyCount = 0;
                    if (value.Type != default) propertyCount++;
                    if (value.Signature != default) propertyCount++;
                    if (value.CreationTime != default) propertyCount++;
                    if (value.Clue != default) propertyCount++;
                    if (value.Certificate != default) propertyCount++;
                    w.Write(propertyCount);
                }

                // Type
                if (value.Type != default)
                {
                    w.Write((uint)0);
                    w.Write(value.Type);
                }
                // Signature
                if (value.Signature != default)
                {
                    w.Write((uint)1);
                    OmniSignature.Formatter.Serialize(w, value.Signature, rank + 1);
                }
                // CreationTime
                if (value.CreationTime != default)
                {
                    w.Write((uint)2);
                    w.Write(value.CreationTime);
                }
                // Clue
                if (value.Clue != default)
                {
                    w.Write((uint)3);
                    Clue.Formatter.Serialize(w, value.Clue, rank + 1);
                }
                // Certificate
                if (value.Certificate != default)
                {
                    w.Write((uint)4);
                    OmniCertificate.Formatter.Serialize(w, value.Certificate, rank + 1);
                }
            }

            public UnicastClue Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Read property count
                uint propertyCount = r.GetUInt32();

                string p_type = default;
                OmniSignature p_signature = default;
                Timestamp p_creationTime = default;
                Clue p_clue = default;
                OmniCertificate p_certificate = default;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0: // Type
                            {
                                p_type = r.GetString(256);
                                break;
                            }
                        case 1: // Signature
                            {
                                p_signature = OmniSignature.Formatter.Deserialize(r, rank + 1);
                                break;
                            }
                        case 2: // CreationTime
                            {
                                p_creationTime = r.GetTimestamp();
                                break;
                            }
                        case 3: // Clue
                            {
                                p_clue = Clue.Formatter.Deserialize(r, rank + 1);
                                break;
                            }
                        case 4: // Certificate
                            {
                                p_certificate = OmniCertificate.Formatter.Deserialize(r, rank + 1);
                                break;
                            }
                    }
                }

                return new UnicastClue(p_type, p_signature, p_creationTime, p_clue, p_certificate);
            }
        }
    }

    internal sealed partial class MulticastClue : RocketPackMessageBase<MulticastClue>
    {
        static MulticastClue()
        {
            MulticastClue.Formatter = new CustomFormatter();
        }

        public static readonly int MaxTypeLength = 256;

        public MulticastClue(string type, Channel channel, Timestamp creationTime, Clue clue, OmniHashcash hashcash, OmniCertificate certificate)
        {
            if (type is null) throw new ArgumentNullException("type");
            if (type.Length > 256) throw new ArgumentOutOfRangeException("type");
            if (channel is null) throw new ArgumentNullException("channel");
            if (clue is null) throw new ArgumentNullException("clue");
            if (hashcash is null) throw new ArgumentNullException("hashcash");
            if (certificate is null) throw new ArgumentNullException("certificate");

            this.Type = type;
            this.Channel = channel;
            this.CreationTime = creationTime;
            this.Clue = clue;
            this.Hashcash = hashcash;
            this.Certificate = certificate;

            {
                var hashCode = new HashCode();
                if (this.Type != default) hashCode.Add(this.Type.GetHashCode());
                if (this.Channel != default) hashCode.Add(this.Channel.GetHashCode());
                if (this.CreationTime != default) hashCode.Add(this.CreationTime.GetHashCode());
                if (this.Clue != default) hashCode.Add(this.Clue.GetHashCode());
                if (this.Hashcash != default) hashCode.Add(this.Hashcash.GetHashCode());
                if (this.Certificate != default) hashCode.Add(this.Certificate.GetHashCode());
                _hashCode = hashCode.ToHashCode();
            }
        }

        public string Type { get; }
        public Channel Channel { get; }
        public Timestamp CreationTime { get; }
        public Clue Clue { get; }
        public OmniHashcash Hashcash { get; }
        public OmniCertificate Certificate { get; }

        public override bool Equals(MulticastClue target)
        {
            if ((object)target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;
            if (this.Type != target.Type) return false;
            if (this.Channel != target.Channel) return false;
            if (this.CreationTime != target.CreationTime) return false;
            if (this.Clue != target.Clue) return false;
            if (this.Hashcash != target.Hashcash) return false;
            if (this.Certificate != target.Certificate) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        private sealed class CustomFormatter : IRocketPackFormatter<MulticastClue>
        {
            public void Serialize(RocketPackWriter w, MulticastClue value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Write property count
                {
                    uint propertyCount = 0;
                    if (value.Type != default) propertyCount++;
                    if (value.Channel != default) propertyCount++;
                    if (value.CreationTime != default) propertyCount++;
                    if (value.Clue != default) propertyCount++;
                    if (value.Hashcash != default) propertyCount++;
                    if (value.Certificate != default) propertyCount++;
                    w.Write(propertyCount);
                }

                // Type
                if (value.Type != default)
                {
                    w.Write((uint)0);
                    w.Write(value.Type);
                }
                // Channel
                if (value.Channel != default)
                {
                    w.Write((uint)1);
                    Channel.Formatter.Serialize(w, value.Channel, rank + 1);
                }
                // CreationTime
                if (value.CreationTime != default)
                {
                    w.Write((uint)2);
                    w.Write(value.CreationTime);
                }
                // Clue
                if (value.Clue != default)
                {
                    w.Write((uint)3);
                    Clue.Formatter.Serialize(w, value.Clue, rank + 1);
                }
                // Hashcash
                if (value.Hashcash != default)
                {
                    w.Write((uint)4);
                    OmniHashcash.Formatter.Serialize(w, value.Hashcash, rank + 1);
                }
                // Certificate
                if (value.Certificate != default)
                {
                    w.Write((uint)5);
                    OmniCertificate.Formatter.Serialize(w, value.Certificate, rank + 1);
                }
            }

            public MulticastClue Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Read property count
                uint propertyCount = r.GetUInt32();

                string p_type = default;
                Channel p_channel = default;
                Timestamp p_creationTime = default;
                Clue p_clue = default;
                OmniHashcash p_hashcash = default;
                OmniCertificate p_certificate = default;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0: // Type
                            {
                                p_type = r.GetString(256);
                                break;
                            }
                        case 1: // Channel
                            {
                                p_channel = Channel.Formatter.Deserialize(r, rank + 1);
                                break;
                            }
                        case 2: // CreationTime
                            {
                                p_creationTime = r.GetTimestamp();
                                break;
                            }
                        case 3: // Clue
                            {
                                p_clue = Clue.Formatter.Deserialize(r, rank + 1);
                                break;
                            }
                        case 4: // Hashcash
                            {
                                p_hashcash = OmniHashcash.Formatter.Deserialize(r, rank + 1);
                                break;
                            }
                        case 5: // Certificate
                            {
                                p_certificate = OmniCertificate.Formatter.Deserialize(r, rank + 1);
                                break;
                            }
                    }
                }

                return new MulticastClue(p_type, p_channel, p_creationTime, p_clue, p_hashcash, p_certificate);
            }
        }
    }

    internal sealed partial class HelloMessage : RocketPackMessageBase<HelloMessage>
    {
        static HelloMessage()
        {
            HelloMessage.Formatter = new CustomFormatter();
        }

        public static readonly int MaxProtocolVersionsCount = 32;

        public HelloMessage(IList<ProtocolVersion> protocolVersions)
        {
            if (protocolVersions is null) throw new ArgumentNullException("protocolVersions");
            if (protocolVersions.Count > 32) throw new ArgumentOutOfRangeException("protocolVersions");

            this.ProtocolVersions = new ReadOnlyCollection<ProtocolVersion>(protocolVersions);

            {
                var hashCode = new HashCode();
                foreach (var n in this.ProtocolVersions)
                {
                    if (n != default) hashCode.Add(n.GetHashCode());
                }
                _hashCode = hashCode.ToHashCode();
            }
        }

        public IReadOnlyList<ProtocolVersion> ProtocolVersions { get; }

        public override bool Equals(HelloMessage target)
        {
            if ((object)target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;
            if ((this.ProtocolVersions is null) != (target.ProtocolVersions is null)) return false;
            if (!(this.ProtocolVersions is null) && !(target.ProtocolVersions is null) && !CollectionHelper.Equals(this.ProtocolVersions, target.ProtocolVersions)) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        private sealed class CustomFormatter : IRocketPackFormatter<HelloMessage>
        {
            public void Serialize(RocketPackWriter w, HelloMessage value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Write property count
                {
                    uint propertyCount = 0;
                    if (value.ProtocolVersions.Count != 0) propertyCount++;
                    w.Write(propertyCount);
                }

                // ProtocolVersions
                if (value.ProtocolVersions.Count != 0)
                {
                    w.Write((uint)0);
                    w.Write((uint)value.ProtocolVersions.Count);
                    foreach (var n in value.ProtocolVersions)
                    {
                        w.Write((ulong)n);
                    }
                }
            }

            public HelloMessage Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Read property count
                uint propertyCount = r.GetUInt32();

                IList<ProtocolVersion> p_protocolVersions = default;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0: // ProtocolVersions
                            {
                                var length = r.GetUInt32();
                                p_protocolVersions = new ProtocolVersion[length];
                                for (int i = 0; i < p_protocolVersions.Count; i++)
                                {
                                    p_protocolVersions[i] = (ProtocolVersion)r.GetUInt64();
                                }
                                break;
                            }
                    }
                }

                return new HelloMessage(p_protocolVersions);
            }
        }
    }

    internal sealed partial class ProfileMessage : RocketPackMessageBase<ProfileMessage>
    {
        static ProfileMessage()
        {
            ProfileMessage.Formatter = new CustomFormatter();
        }

        public static readonly int MaxIdLength = 32;

        public ProfileMessage(ReadOnlyMemory<byte> id, OmniAddress address)
        {
            if (id.Length > 32) throw new ArgumentOutOfRangeException("id");
            if (address is null) throw new ArgumentNullException("address");

            this.Id = id;
            this.Address = address;

            {
                var hashCode = new HashCode();
                if (!this.Id.IsEmpty) hashCode.Add(ObjectHelper.GetHashCode(this.Id.Span));
                if (this.Address != default) hashCode.Add(this.Address.GetHashCode());
                _hashCode = hashCode.ToHashCode();
            }
        }

        public ReadOnlyMemory<byte> Id { get; }
        public OmniAddress Address { get; }

        public override bool Equals(ProfileMessage target)
        {
            if ((object)target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;
            if (!BytesOperations.SequenceEqual(this.Id.Span, target.Id.Span)) return false;
            if (this.Address != target.Address) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        private sealed class CustomFormatter : IRocketPackFormatter<ProfileMessage>
        {
            public void Serialize(RocketPackWriter w, ProfileMessage value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Write property count
                {
                    uint propertyCount = 0;
                    if (!value.Id.IsEmpty) propertyCount++;
                    if (value.Address != default) propertyCount++;
                    w.Write(propertyCount);
                }

                // Id
                if (!value.Id.IsEmpty)
                {
                    w.Write((uint)0);
                    w.Write(value.Id.Span);
                }
                // Address
                if (value.Address != default)
                {
                    w.Write((uint)1);
                    OmniAddress.Formatter.Serialize(w, value.Address, rank + 1);
                }
            }

            public ProfileMessage Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Read property count
                uint propertyCount = r.GetUInt32();

                ReadOnlyMemory<byte> p_id = default;
                OmniAddress p_address = default;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0: // Id
                            {
                                p_id = r.GetMemory(32);
                                break;
                            }
                        case 1: // Address
                            {
                                p_address = OmniAddress.Formatter.Deserialize(r, rank + 1);
                                break;
                            }
                    }
                }

                return new ProfileMessage(p_id, p_address);
            }
        }
    }

    internal sealed partial class NodeAddressesMessage : RocketPackMessageBase<NodeAddressesMessage>
    {
        static NodeAddressesMessage()
        {
            NodeAddressesMessage.Formatter = new CustomFormatter();
        }

        public static readonly int MaxAddressesCount = 256;

        public NodeAddressesMessage(IList<OmniAddress> addresses)
        {
            if (addresses is null) throw new ArgumentNullException("addresses");
            if (addresses.Count > 256) throw new ArgumentOutOfRangeException("addresses");
            foreach (var n in addresses)
            {
                if (n is null) throw new ArgumentNullException("n");
            }

            this.Addresses = new ReadOnlyCollection<OmniAddress>(addresses);

            {
                var hashCode = new HashCode();
                foreach (var n in this.Addresses)
                {
                    if (n != default) hashCode.Add(n.GetHashCode());
                }
                _hashCode = hashCode.ToHashCode();
            }
        }

        public IReadOnlyList<OmniAddress> Addresses { get; }

        public override bool Equals(NodeAddressesMessage target)
        {
            if ((object)target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;
            if ((this.Addresses is null) != (target.Addresses is null)) return false;
            if (!(this.Addresses is null) && !(target.Addresses is null) && !CollectionHelper.Equals(this.Addresses, target.Addresses)) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        private sealed class CustomFormatter : IRocketPackFormatter<NodeAddressesMessage>
        {
            public void Serialize(RocketPackWriter w, NodeAddressesMessage value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Write property count
                {
                    uint propertyCount = 0;
                    if (value.Addresses.Count != 0) propertyCount++;
                    w.Write(propertyCount);
                }

                // Addresses
                if (value.Addresses.Count != 0)
                {
                    w.Write((uint)0);
                    w.Write((uint)value.Addresses.Count);
                    foreach (var n in value.Addresses)
                    {
                        OmniAddress.Formatter.Serialize(w, n, rank + 1);
                    }
                }
            }

            public NodeAddressesMessage Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Read property count
                uint propertyCount = r.GetUInt32();

                IList<OmniAddress> p_addresses = default;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0: // Addresses
                            {
                                var length = r.GetUInt32();
                                p_addresses = new OmniAddress[length];
                                for (int i = 0; i < p_addresses.Count; i++)
                                {
                                    p_addresses[i] = OmniAddress.Formatter.Deserialize(r, rank + 1);
                                }
                                break;
                            }
                    }
                }

                return new NodeAddressesMessage(p_addresses);
            }
        }
    }

    internal sealed partial class RelayOption : RocketPackMessageBase<RelayOption>
    {
        static RelayOption()
        {
            RelayOption.Formatter = new CustomFormatter();
        }

        public RelayOption(byte hopLimit, byte priority)
        {
            this.HopLimit = hopLimit;
            this.Priority = priority;

            {
                var hashCode = new HashCode();
                if (this.HopLimit != default) hashCode.Add(this.HopLimit.GetHashCode());
                if (this.Priority != default) hashCode.Add(this.Priority.GetHashCode());
                _hashCode = hashCode.ToHashCode();
            }
        }

        public byte HopLimit { get; }
        public byte Priority { get; }

        public override bool Equals(RelayOption target)
        {
            if ((object)target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;
            if (this.HopLimit != target.HopLimit) return false;
            if (this.Priority != target.Priority) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        private sealed class CustomFormatter : IRocketPackFormatter<RelayOption>
        {
            public void Serialize(RocketPackWriter w, RelayOption value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Write property count
                {
                    uint propertyCount = 0;
                    if (value.HopLimit != default) propertyCount++;
                    if (value.Priority != default) propertyCount++;
                    w.Write(propertyCount);
                }

                // HopLimit
                if (value.HopLimit != default)
                {
                    w.Write((uint)0);
                    w.Write(value.HopLimit);
                }
                // Priority
                if (value.Priority != default)
                {
                    w.Write((uint)1);
                    w.Write(value.Priority);
                }
            }

            public RelayOption Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Read property count
                uint propertyCount = r.GetUInt32();

                byte p_hopLimit = default;
                byte p_priority = default;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0: // HopLimit
                            {
                                p_hopLimit = r.GetUInt8();
                                break;
                            }
                        case 1: // Priority
                            {
                                p_priority = r.GetUInt8();
                                break;
                            }
                    }
                }

                return new RelayOption(p_hopLimit, p_priority);
            }
        }
    }

    internal sealed partial class WantBroadcastCluesMessage : RocketPackMessageBase<WantBroadcastCluesMessage>
    {
        static WantBroadcastCluesMessage()
        {
            WantBroadcastCluesMessage.Formatter = new CustomFormatter();
        }

        public static readonly int MaxParametersCount = 8192;

        public WantBroadcastCluesMessage(IDictionary<OmniSignature, RelayOption> parameters)
        {
            if (parameters is null) throw new ArgumentNullException("parameters");
            if (parameters.Count > 8192) throw new ArgumentOutOfRangeException("parameters");
            foreach (var n in parameters)
            {
                if (n.Key is null) throw new ArgumentNullException("n.Key");
                if (n.Value is null) throw new ArgumentNullException("n.Value");
            }

            this.Parameters = new ReadOnlyDictionary<OmniSignature, RelayOption>(parameters);

            {
                var hashCode = new HashCode();
                foreach (var n in this.Parameters)
                {
                    if (n.Key != default) hashCode.Add(n.Key.GetHashCode());
                    if (n.Value != default) hashCode.Add(n.Value.GetHashCode());
                }
                _hashCode = hashCode.ToHashCode();
            }
        }

        public IReadOnlyDictionary<OmniSignature, RelayOption> Parameters { get; }

        public override bool Equals(WantBroadcastCluesMessage target)
        {
            if ((object)target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;
            if ((this.Parameters is null) != (target.Parameters is null)) return false;
            if (!(this.Parameters is null) && !(target.Parameters is null) && !CollectionHelper.Equals(this.Parameters, target.Parameters)) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        private sealed class CustomFormatter : IRocketPackFormatter<WantBroadcastCluesMessage>
        {
            public void Serialize(RocketPackWriter w, WantBroadcastCluesMessage value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Write property count
                {
                    uint propertyCount = 0;
                    if (value.Parameters.Count != 0) propertyCount++;
                    w.Write(propertyCount);
                }

                // Parameters
                if (value.Parameters.Count != 0)
                {
                    w.Write((uint)0);
                    w.Write((uint)value.Parameters.Count);
                    foreach (var n in value.Parameters)
                    {
                        OmniSignature.Formatter.Serialize(w, n.Key, rank + 1);
                        RelayOption.Formatter.Serialize(w, n.Value, rank + 1);
                    }
                }
            }

            public WantBroadcastCluesMessage Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Read property count
                uint propertyCount = r.GetUInt32();

                IDictionary<OmniSignature, RelayOption> p_parameters = default;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0: // Parameters
                            {
                                var length = r.GetUInt32();
                                p_parameters = new Dictionary<OmniSignature, RelayOption>();
                                OmniSignature t_key = default;
                                RelayOption t_value = default;
                                for (int i = 0; i < length; i++)
                                {
                                    t_key = OmniSignature.Formatter.Deserialize(r, rank + 1);
                                    t_value = RelayOption.Formatter.Deserialize(r, rank + 1);
                                    p_parameters[t_key] = t_value;
                                }
                                break;
                            }
                    }
                }

                return new WantBroadcastCluesMessage(p_parameters);
            }
        }
    }

    internal sealed partial class WantUnicastCluesMessage : RocketPackMessageBase<WantUnicastCluesMessage>
    {
        static WantUnicastCluesMessage()
        {
            WantUnicastCluesMessage.Formatter = new CustomFormatter();
        }

        public static readonly int MaxParametersCount = 8192;

        public WantUnicastCluesMessage(IDictionary<OmniSignature, RelayOption> parameters)
        {
            if (parameters is null) throw new ArgumentNullException("parameters");
            if (parameters.Count > 8192) throw new ArgumentOutOfRangeException("parameters");
            foreach (var n in parameters)
            {
                if (n.Key is null) throw new ArgumentNullException("n.Key");
                if (n.Value is null) throw new ArgumentNullException("n.Value");
            }

            this.Parameters = new ReadOnlyDictionary<OmniSignature, RelayOption>(parameters);

            {
                var hashCode = new HashCode();
                foreach (var n in this.Parameters)
                {
                    if (n.Key != default) hashCode.Add(n.Key.GetHashCode());
                    if (n.Value != default) hashCode.Add(n.Value.GetHashCode());
                }
                _hashCode = hashCode.ToHashCode();
            }
        }

        public IReadOnlyDictionary<OmniSignature, RelayOption> Parameters { get; }

        public override bool Equals(WantUnicastCluesMessage target)
        {
            if ((object)target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;
            if ((this.Parameters is null) != (target.Parameters is null)) return false;
            if (!(this.Parameters is null) && !(target.Parameters is null) && !CollectionHelper.Equals(this.Parameters, target.Parameters)) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        private sealed class CustomFormatter : IRocketPackFormatter<WantUnicastCluesMessage>
        {
            public void Serialize(RocketPackWriter w, WantUnicastCluesMessage value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Write property count
                {
                    uint propertyCount = 0;
                    if (value.Parameters.Count != 0) propertyCount++;
                    w.Write(propertyCount);
                }

                // Parameters
                if (value.Parameters.Count != 0)
                {
                    w.Write((uint)0);
                    w.Write((uint)value.Parameters.Count);
                    foreach (var n in value.Parameters)
                    {
                        OmniSignature.Formatter.Serialize(w, n.Key, rank + 1);
                        RelayOption.Formatter.Serialize(w, n.Value, rank + 1);
                    }
                }
            }

            public WantUnicastCluesMessage Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Read property count
                uint propertyCount = r.GetUInt32();

                IDictionary<OmniSignature, RelayOption> p_parameters = default;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0: // Parameters
                            {
                                var length = r.GetUInt32();
                                p_parameters = new Dictionary<OmniSignature, RelayOption>();
                                OmniSignature t_key = default;
                                RelayOption t_value = default;
                                for (int i = 0; i < length; i++)
                                {
                                    t_key = OmniSignature.Formatter.Deserialize(r, rank + 1);
                                    t_value = RelayOption.Formatter.Deserialize(r, rank + 1);
                                    p_parameters[t_key] = t_value;
                                }
                                break;
                            }
                    }
                }

                return new WantUnicastCluesMessage(p_parameters);
            }
        }
    }

    internal sealed partial class WantMulticastCluesMessage : RocketPackMessageBase<WantMulticastCluesMessage>
    {
        static WantMulticastCluesMessage()
        {
            WantMulticastCluesMessage.Formatter = new CustomFormatter();
        }

        public static readonly int MaxParametersCount = 8192;

        public WantMulticastCluesMessage(IDictionary<Channel, RelayOption> parameters)
        {
            if (parameters is null) throw new ArgumentNullException("parameters");
            if (parameters.Count > 8192) throw new ArgumentOutOfRangeException("parameters");
            foreach (var n in parameters)
            {
                if (n.Key is null) throw new ArgumentNullException("n.Key");
                if (n.Value is null) throw new ArgumentNullException("n.Value");
            }

            this.Parameters = new ReadOnlyDictionary<Channel, RelayOption>(parameters);

            {
                var hashCode = new HashCode();
                foreach (var n in this.Parameters)
                {
                    if (n.Key != default) hashCode.Add(n.Key.GetHashCode());
                    if (n.Value != default) hashCode.Add(n.Value.GetHashCode());
                }
                _hashCode = hashCode.ToHashCode();
            }
        }

        public IReadOnlyDictionary<Channel, RelayOption> Parameters { get; }

        public override bool Equals(WantMulticastCluesMessage target)
        {
            if ((object)target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;
            if ((this.Parameters is null) != (target.Parameters is null)) return false;
            if (!(this.Parameters is null) && !(target.Parameters is null) && !CollectionHelper.Equals(this.Parameters, target.Parameters)) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        private sealed class CustomFormatter : IRocketPackFormatter<WantMulticastCluesMessage>
        {
            public void Serialize(RocketPackWriter w, WantMulticastCluesMessage value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Write property count
                {
                    uint propertyCount = 0;
                    if (value.Parameters.Count != 0) propertyCount++;
                    w.Write(propertyCount);
                }

                // Parameters
                if (value.Parameters.Count != 0)
                {
                    w.Write((uint)0);
                    w.Write((uint)value.Parameters.Count);
                    foreach (var n in value.Parameters)
                    {
                        Channel.Formatter.Serialize(w, n.Key, rank + 1);
                        RelayOption.Formatter.Serialize(w, n.Value, rank + 1);
                    }
                }
            }

            public WantMulticastCluesMessage Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Read property count
                uint propertyCount = r.GetUInt32();

                IDictionary<Channel, RelayOption> p_parameters = default;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0: // Parameters
                            {
                                var length = r.GetUInt32();
                                p_parameters = new Dictionary<Channel, RelayOption>();
                                Channel t_key = default;
                                RelayOption t_value = default;
                                for (int i = 0; i < length; i++)
                                {
                                    t_key = Channel.Formatter.Deserialize(r, rank + 1);
                                    t_value = RelayOption.Formatter.Deserialize(r, rank + 1);
                                    p_parameters[t_key] = t_value;
                                }
                                break;
                            }
                    }
                }

                return new WantMulticastCluesMessage(p_parameters);
            }
        }
    }

    internal sealed partial class BroadcastCluesMessage : RocketPackMessageBase<BroadcastCluesMessage>
    {
        static BroadcastCluesMessage()
        {
            BroadcastCluesMessage.Formatter = new CustomFormatter();
        }

        public static readonly int MaxResultsCount = 8192;

        public BroadcastCluesMessage(IList<BroadcastClue> results)
        {
            if (results is null) throw new ArgumentNullException("results");
            if (results.Count > 8192) throw new ArgumentOutOfRangeException("results");
            foreach (var n in results)
            {
                if (n is null) throw new ArgumentNullException("n");
            }

            this.Results = new ReadOnlyCollection<BroadcastClue>(results);

            {
                var hashCode = new HashCode();
                foreach (var n in this.Results)
                {
                    if (n != default) hashCode.Add(n.GetHashCode());
                }
                _hashCode = hashCode.ToHashCode();
            }
        }

        public IReadOnlyList<BroadcastClue> Results { get; }

        public override bool Equals(BroadcastCluesMessage target)
        {
            if ((object)target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;
            if ((this.Results is null) != (target.Results is null)) return false;
            if (!(this.Results is null) && !(target.Results is null) && !CollectionHelper.Equals(this.Results, target.Results)) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        private sealed class CustomFormatter : IRocketPackFormatter<BroadcastCluesMessage>
        {
            public void Serialize(RocketPackWriter w, BroadcastCluesMessage value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Write property count
                {
                    uint propertyCount = 0;
                    if (value.Results.Count != 0) propertyCount++;
                    w.Write(propertyCount);
                }

                // Results
                if (value.Results.Count != 0)
                {
                    w.Write((uint)0);
                    w.Write((uint)value.Results.Count);
                    foreach (var n in value.Results)
                    {
                        BroadcastClue.Formatter.Serialize(w, n, rank + 1);
                    }
                }
            }

            public BroadcastCluesMessage Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Read property count
                uint propertyCount = r.GetUInt32();

                IList<BroadcastClue> p_results = default;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0: // Results
                            {
                                var length = r.GetUInt32();
                                p_results = new BroadcastClue[length];
                                for (int i = 0; i < p_results.Count; i++)
                                {
                                    p_results[i] = BroadcastClue.Formatter.Deserialize(r, rank + 1);
                                }
                                break;
                            }
                    }
                }

                return new BroadcastCluesMessage(p_results);
            }
        }
    }

    internal sealed partial class UnicastCluesMessage : RocketPackMessageBase<UnicastCluesMessage>
    {
        static UnicastCluesMessage()
        {
            UnicastCluesMessage.Formatter = new CustomFormatter();
        }

        public static readonly int MaxResultsCount = 8192;

        public UnicastCluesMessage(IList<UnicastClue> results)
        {
            if (results is null) throw new ArgumentNullException("results");
            if (results.Count > 8192) throw new ArgumentOutOfRangeException("results");
            foreach (var n in results)
            {
                if (n is null) throw new ArgumentNullException("n");
            }

            this.Results = new ReadOnlyCollection<UnicastClue>(results);

            {
                var hashCode = new HashCode();
                foreach (var n in this.Results)
                {
                    if (n != default) hashCode.Add(n.GetHashCode());
                }
                _hashCode = hashCode.ToHashCode();
            }
        }

        public IReadOnlyList<UnicastClue> Results { get; }

        public override bool Equals(UnicastCluesMessage target)
        {
            if ((object)target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;
            if ((this.Results is null) != (target.Results is null)) return false;
            if (!(this.Results is null) && !(target.Results is null) && !CollectionHelper.Equals(this.Results, target.Results)) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        private sealed class CustomFormatter : IRocketPackFormatter<UnicastCluesMessage>
        {
            public void Serialize(RocketPackWriter w, UnicastCluesMessage value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Write property count
                {
                    uint propertyCount = 0;
                    if (value.Results.Count != 0) propertyCount++;
                    w.Write(propertyCount);
                }

                // Results
                if (value.Results.Count != 0)
                {
                    w.Write((uint)0);
                    w.Write((uint)value.Results.Count);
                    foreach (var n in value.Results)
                    {
                        UnicastClue.Formatter.Serialize(w, n, rank + 1);
                    }
                }
            }

            public UnicastCluesMessage Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Read property count
                uint propertyCount = r.GetUInt32();

                IList<UnicastClue> p_results = default;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0: // Results
                            {
                                var length = r.GetUInt32();
                                p_results = new UnicastClue[length];
                                for (int i = 0; i < p_results.Count; i++)
                                {
                                    p_results[i] = UnicastClue.Formatter.Deserialize(r, rank + 1);
                                }
                                break;
                            }
                    }
                }

                return new UnicastCluesMessage(p_results);
            }
        }
    }

    internal sealed partial class MulticastCluesMessage : RocketPackMessageBase<MulticastCluesMessage>
    {
        static MulticastCluesMessage()
        {
            MulticastCluesMessage.Formatter = new CustomFormatter();
        }

        public static readonly int MaxResultsCount = 8192;

        public MulticastCluesMessage(IList<MulticastClue> results)
        {
            if (results is null) throw new ArgumentNullException("results");
            if (results.Count > 8192) throw new ArgumentOutOfRangeException("results");
            foreach (var n in results)
            {
                if (n is null) throw new ArgumentNullException("n");
            }

            this.Results = new ReadOnlyCollection<MulticastClue>(results);

            {
                var hashCode = new HashCode();
                foreach (var n in this.Results)
                {
                    if (n != default) hashCode.Add(n.GetHashCode());
                }
                _hashCode = hashCode.ToHashCode();
            }
        }

        public IReadOnlyList<MulticastClue> Results { get; }

        public override bool Equals(MulticastCluesMessage target)
        {
            if ((object)target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;
            if ((this.Results is null) != (target.Results is null)) return false;
            if (!(this.Results is null) && !(target.Results is null) && !CollectionHelper.Equals(this.Results, target.Results)) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        private sealed class CustomFormatter : IRocketPackFormatter<MulticastCluesMessage>
        {
            public void Serialize(RocketPackWriter w, MulticastCluesMessage value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Write property count
                {
                    uint propertyCount = 0;
                    if (value.Results.Count != 0) propertyCount++;
                    w.Write(propertyCount);
                }

                // Results
                if (value.Results.Count != 0)
                {
                    w.Write((uint)0);
                    w.Write((uint)value.Results.Count);
                    foreach (var n in value.Results)
                    {
                        MulticastClue.Formatter.Serialize(w, n, rank + 1);
                    }
                }
            }

            public MulticastCluesMessage Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Read property count
                uint propertyCount = r.GetUInt32();

                IList<MulticastClue> p_results = default;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0: // Results
                            {
                                var length = r.GetUInt32();
                                p_results = new MulticastClue[length];
                                for (int i = 0; i < p_results.Count; i++)
                                {
                                    p_results[i] = MulticastClue.Formatter.Deserialize(r, rank + 1);
                                }
                                break;
                            }
                    }
                }

                return new MulticastCluesMessage(p_results);
            }
        }
    }

    internal sealed partial class PublishBlocksMessage : RocketPackMessageBase<PublishBlocksMessage>
    {
        static PublishBlocksMessage()
        {
            PublishBlocksMessage.Formatter = new CustomFormatter();
        }

        public static readonly int MaxParametersCount = 8192;

        public PublishBlocksMessage(IDictionary<OmniHash, RelayOption> parameters)
        {
            if (parameters is null) throw new ArgumentNullException("parameters");
            if (parameters.Count > 8192) throw new ArgumentOutOfRangeException("parameters");
            foreach (var n in parameters)
            {
                if (n.Value is null) throw new ArgumentNullException("n.Value");
            }

            this.Parameters = new ReadOnlyDictionary<OmniHash, RelayOption>(parameters);

            {
                var hashCode = new HashCode();
                foreach (var n in this.Parameters)
                {
                    if (n.Key != default) hashCode.Add(n.Key.GetHashCode());
                    if (n.Value != default) hashCode.Add(n.Value.GetHashCode());
                }
                _hashCode = hashCode.ToHashCode();
            }
        }

        public IReadOnlyDictionary<OmniHash, RelayOption> Parameters { get; }

        public override bool Equals(PublishBlocksMessage target)
        {
            if ((object)target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;
            if ((this.Parameters is null) != (target.Parameters is null)) return false;
            if (!(this.Parameters is null) && !(target.Parameters is null) && !CollectionHelper.Equals(this.Parameters, target.Parameters)) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        private sealed class CustomFormatter : IRocketPackFormatter<PublishBlocksMessage>
        {
            public void Serialize(RocketPackWriter w, PublishBlocksMessage value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Write property count
                {
                    uint propertyCount = 0;
                    if (value.Parameters.Count != 0) propertyCount++;
                    w.Write(propertyCount);
                }

                // Parameters
                if (value.Parameters.Count != 0)
                {
                    w.Write((uint)0);
                    w.Write((uint)value.Parameters.Count);
                    foreach (var n in value.Parameters)
                    {
                        OmniHash.Formatter.Serialize(w, n.Key, rank + 1);
                        RelayOption.Formatter.Serialize(w, n.Value, rank + 1);
                    }
                }
            }

            public PublishBlocksMessage Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Read property count
                uint propertyCount = r.GetUInt32();

                IDictionary<OmniHash, RelayOption> p_parameters = default;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0: // Parameters
                            {
                                var length = r.GetUInt32();
                                p_parameters = new Dictionary<OmniHash, RelayOption>();
                                OmniHash t_key = default;
                                RelayOption t_value = default;
                                for (int i = 0; i < length; i++)
                                {
                                    t_key = OmniHash.Formatter.Deserialize(r, rank + 1);
                                    t_value = RelayOption.Formatter.Deserialize(r, rank + 1);
                                    p_parameters[t_key] = t_value;
                                }
                                break;
                            }
                    }
                }

                return new PublishBlocksMessage(p_parameters);
            }
        }
    }

    internal sealed partial class WantBlocksMessage : RocketPackMessageBase<WantBlocksMessage>
    {
        static WantBlocksMessage()
        {
            WantBlocksMessage.Formatter = new CustomFormatter();
        }

        public static readonly int MaxParametersCount = 8192;

        public WantBlocksMessage(IDictionary<OmniHash, RelayOption> parameters)
        {
            if (parameters is null) throw new ArgumentNullException("parameters");
            if (parameters.Count > 8192) throw new ArgumentOutOfRangeException("parameters");
            foreach (var n in parameters)
            {
                if (n.Value is null) throw new ArgumentNullException("n.Value");
            }

            this.Parameters = new ReadOnlyDictionary<OmniHash, RelayOption>(parameters);

            {
                var hashCode = new HashCode();
                foreach (var n in this.Parameters)
                {
                    if (n.Key != default) hashCode.Add(n.Key.GetHashCode());
                    if (n.Value != default) hashCode.Add(n.Value.GetHashCode());
                }
                _hashCode = hashCode.ToHashCode();
            }
        }

        public IReadOnlyDictionary<OmniHash, RelayOption> Parameters { get; }

        public override bool Equals(WantBlocksMessage target)
        {
            if ((object)target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;
            if ((this.Parameters is null) != (target.Parameters is null)) return false;
            if (!(this.Parameters is null) && !(target.Parameters is null) && !CollectionHelper.Equals(this.Parameters, target.Parameters)) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        private sealed class CustomFormatter : IRocketPackFormatter<WantBlocksMessage>
        {
            public void Serialize(RocketPackWriter w, WantBlocksMessage value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Write property count
                {
                    uint propertyCount = 0;
                    if (value.Parameters.Count != 0) propertyCount++;
                    w.Write(propertyCount);
                }

                // Parameters
                if (value.Parameters.Count != 0)
                {
                    w.Write((uint)0);
                    w.Write((uint)value.Parameters.Count);
                    foreach (var n in value.Parameters)
                    {
                        OmniHash.Formatter.Serialize(w, n.Key, rank + 1);
                        RelayOption.Formatter.Serialize(w, n.Value, rank + 1);
                    }
                }
            }

            public WantBlocksMessage Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Read property count
                uint propertyCount = r.GetUInt32();

                IDictionary<OmniHash, RelayOption> p_parameters = default;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0: // Parameters
                            {
                                var length = r.GetUInt32();
                                p_parameters = new Dictionary<OmniHash, RelayOption>();
                                OmniHash t_key = default;
                                RelayOption t_value = default;
                                for (int i = 0; i < length; i++)
                                {
                                    t_key = OmniHash.Formatter.Deserialize(r, rank + 1);
                                    t_value = RelayOption.Formatter.Deserialize(r, rank + 1);
                                    p_parameters[t_key] = t_value;
                                }
                                break;
                            }
                    }
                }

                return new WantBlocksMessage(p_parameters);
            }
        }
    }

    internal sealed partial class DiffuseBlockMessage : RocketPackMessageBase<DiffuseBlockMessage>, IDisposable
    {
        static DiffuseBlockMessage()
        {
            DiffuseBlockMessage.Formatter = new CustomFormatter();
        }

        public static readonly int MaxValueLength = 4194304;

        public DiffuseBlockMessage(OmniHash hash, RelayOption relayOption, IMemoryOwner<byte> value)
        {
            if (relayOption is null) throw new ArgumentNullException("relayOption");
            if (value is null) throw new ArgumentNullException("value");
            if (value.Memory.Length > 4194304) throw new ArgumentOutOfRangeException("value");

            this.Hash = hash;
            this.RelayOption = relayOption;
            _value = value;

            {
                var hashCode = new HashCode();
                if (this.Hash != default) hashCode.Add(this.Hash.GetHashCode());
                if (this.RelayOption != default) hashCode.Add(this.RelayOption.GetHashCode());
                if (!this.Value.IsEmpty) hashCode.Add(ObjectHelper.GetHashCode(this.Value.Span));
                _hashCode = hashCode.ToHashCode();
            }
        }

        public OmniHash Hash { get; }
        public RelayOption RelayOption { get; }
        private readonly IMemoryOwner<byte> _value;
        public ReadOnlyMemory<byte> Value => _value.Memory;

        public override bool Equals(DiffuseBlockMessage target)
        {
            if ((object)target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;
            if (this.Hash != target.Hash) return false;
            if (this.RelayOption != target.RelayOption) return false;
            if (!BytesOperations.SequenceEqual(this.Value.Span, target.Value.Span)) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        public void Dispose()
        {
            _value?.Dispose();
        }

        private sealed class CustomFormatter : IRocketPackFormatter<DiffuseBlockMessage>
        {
            public void Serialize(RocketPackWriter w, DiffuseBlockMessage value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Write property count
                {
                    uint propertyCount = 0;
                    if (value.Hash != default) propertyCount++;
                    if (value.RelayOption != default) propertyCount++;
                    if (!value.Value.IsEmpty) propertyCount++;
                    w.Write(propertyCount);
                }

                // Hash
                if (value.Hash != default)
                {
                    w.Write((uint)0);
                    OmniHash.Formatter.Serialize(w, value.Hash, rank + 1);
                }
                // RelayOption
                if (value.RelayOption != default)
                {
                    w.Write((uint)1);
                    RelayOption.Formatter.Serialize(w, value.RelayOption, rank + 1);
                }
                // Value
                if (!value.Value.IsEmpty)
                {
                    w.Write((uint)2);
                    w.Write(value.Value.Span);
                }
            }

            public DiffuseBlockMessage Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Read property count
                uint propertyCount = r.GetUInt32();

                OmniHash p_hash = default;
                RelayOption p_relayOption = default;
                IMemoryOwner<byte> p_value = default;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0: // Hash
                            {
                                p_hash = OmniHash.Formatter.Deserialize(r, rank + 1);
                                break;
                            }
                        case 1: // RelayOption
                            {
                                p_relayOption = RelayOption.Formatter.Deserialize(r, rank + 1);
                                break;
                            }
                        case 2: // Value
                            {
                                p_value = r.GetRecyclableMemory(4194304);
                                break;
                            }
                    }
                }

                return new DiffuseBlockMessage(p_hash, p_relayOption, p_value);
            }
        }
    }

    internal sealed partial class BlockMessage : RocketPackMessageBase<BlockMessage>, IDisposable
    {
        static BlockMessage()
        {
            BlockMessage.Formatter = new CustomFormatter();
        }

        public static readonly int MaxValueLength = 4194304;

        public BlockMessage(OmniHash hash, IMemoryOwner<byte> value)
        {
            if (value is null) throw new ArgumentNullException("value");
            if (value.Memory.Length > 4194304) throw new ArgumentOutOfRangeException("value");

            this.Hash = hash;
            _value = value;

            {
                var hashCode = new HashCode();
                if (this.Hash != default) hashCode.Add(this.Hash.GetHashCode());
                if (!this.Value.IsEmpty) hashCode.Add(ObjectHelper.GetHashCode(this.Value.Span));
                _hashCode = hashCode.ToHashCode();
            }
        }

        public OmniHash Hash { get; }
        private readonly IMemoryOwner<byte> _value;
        public ReadOnlyMemory<byte> Value => _value.Memory;

        public override bool Equals(BlockMessage target)
        {
            if ((object)target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;
            if (this.Hash != target.Hash) return false;
            if (!BytesOperations.SequenceEqual(this.Value.Span, target.Value.Span)) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        public void Dispose()
        {
            _value?.Dispose();
        }

        private sealed class CustomFormatter : IRocketPackFormatter<BlockMessage>
        {
            public void Serialize(RocketPackWriter w, BlockMessage value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Write property count
                {
                    uint propertyCount = 0;
                    if (value.Hash != default) propertyCount++;
                    if (!value.Value.IsEmpty) propertyCount++;
                    w.Write(propertyCount);
                }

                // Hash
                if (value.Hash != default)
                {
                    w.Write((uint)0);
                    OmniHash.Formatter.Serialize(w, value.Hash, rank + 1);
                }
                // Value
                if (!value.Value.IsEmpty)
                {
                    w.Write((uint)1);
                    w.Write(value.Value.Span);
                }
            }

            public BlockMessage Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Read property count
                uint propertyCount = r.GetUInt32();

                OmniHash p_hash = default;
                IMemoryOwner<byte> p_value = default;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0: // Hash
                            {
                                p_hash = OmniHash.Formatter.Deserialize(r, rank + 1);
                                break;
                            }
                        case 1: // Value
                            {
                                p_value = r.GetRecyclableMemory(4194304);
                                break;
                            }
                    }
                }

                return new BlockMessage(p_hash, p_value);
            }
        }
    }

    internal sealed partial class DiffuseBlockInfo : RocketPackMessageBase<DiffuseBlockInfo>
    {
        static DiffuseBlockInfo()
        {
            DiffuseBlockInfo.Formatter = new CustomFormatter();
        }

        public DiffuseBlockInfo(Timestamp creationTime, OmniHash hash, RelayOption relayOption)
        {
            if (relayOption is null) throw new ArgumentNullException("relayOption");

            this.CreationTime = creationTime;
            this.Hash = hash;
            this.RelayOption = relayOption;

            {
                var hashCode = new HashCode();
                if (this.CreationTime != default) hashCode.Add(this.CreationTime.GetHashCode());
                if (this.Hash != default) hashCode.Add(this.Hash.GetHashCode());
                if (this.RelayOption != default) hashCode.Add(this.RelayOption.GetHashCode());
                _hashCode = hashCode.ToHashCode();
            }
        }

        public Timestamp CreationTime { get; }
        public OmniHash Hash { get; }
        public RelayOption RelayOption { get; }

        public override bool Equals(DiffuseBlockInfo target)
        {
            if ((object)target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;
            if (this.CreationTime != target.CreationTime) return false;
            if (this.Hash != target.Hash) return false;
            if (this.RelayOption != target.RelayOption) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        private sealed class CustomFormatter : IRocketPackFormatter<DiffuseBlockInfo>
        {
            public void Serialize(RocketPackWriter w, DiffuseBlockInfo value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Write property count
                {
                    uint propertyCount = 0;
                    if (value.CreationTime != default) propertyCount++;
                    if (value.Hash != default) propertyCount++;
                    if (value.RelayOption != default) propertyCount++;
                    w.Write(propertyCount);
                }

                // CreationTime
                if (value.CreationTime != default)
                {
                    w.Write((uint)0);
                    w.Write(value.CreationTime);
                }
                // Hash
                if (value.Hash != default)
                {
                    w.Write((uint)1);
                    OmniHash.Formatter.Serialize(w, value.Hash, rank + 1);
                }
                // RelayOption
                if (value.RelayOption != default)
                {
                    w.Write((uint)2);
                    RelayOption.Formatter.Serialize(w, value.RelayOption, rank + 1);
                }
            }

            public DiffuseBlockInfo Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Read property count
                uint propertyCount = r.GetUInt32();

                Timestamp p_creationTime = default;
                OmniHash p_hash = default;
                RelayOption p_relayOption = default;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0: // CreationTime
                            {
                                p_creationTime = r.GetTimestamp();
                                break;
                            }
                        case 1: // Hash
                            {
                                p_hash = OmniHash.Formatter.Deserialize(r, rank + 1);
                                break;
                            }
                        case 2: // RelayOption
                            {
                                p_relayOption = RelayOption.Formatter.Deserialize(r, rank + 1);
                                break;
                            }
                    }
                }

                return new DiffuseBlockInfo(p_creationTime, p_hash, p_relayOption);
            }
        }
    }

    internal sealed partial class ExchangeManagerConfig : RocketPackMessageBase<ExchangeManagerConfig>
    {
        static ExchangeManagerConfig()
        {
            ExchangeManagerConfig.Formatter = new CustomFormatter();
        }

        public ExchangeManagerConfig(uint version)
        {
            this.Version = version;

            {
                var hashCode = new HashCode();
                if (this.Version != default) hashCode.Add(this.Version.GetHashCode());
                _hashCode = hashCode.ToHashCode();
            }
        }

        public uint Version { get; }

        public override bool Equals(ExchangeManagerConfig target)
        {
            if ((object)target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;
            if (this.Version != target.Version) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        private sealed class CustomFormatter : IRocketPackFormatter<ExchangeManagerConfig>
        {
            public void Serialize(RocketPackWriter w, ExchangeManagerConfig value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Write property count
                {
                    uint propertyCount = 0;
                    if (value.Version != default) propertyCount++;
                    w.Write(propertyCount);
                }

                // Version
                if (value.Version != default)
                {
                    w.Write((uint)0);
                    w.Write(value.Version);
                }
            }

            public ExchangeManagerConfig Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Read property count
                uint propertyCount = r.GetUInt32();

                uint p_version = default;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0: // Version
                            {
                                p_version = r.GetUInt32();
                                break;
                            }
                    }
                }

                return new ExchangeManagerConfig(p_version);
            }
        }
    }

}
