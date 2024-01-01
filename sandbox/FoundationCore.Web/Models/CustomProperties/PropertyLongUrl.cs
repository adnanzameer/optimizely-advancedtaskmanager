using System.Globalization;
using EPiServer.Core.Transfer;
using EPiServer.PlugIn;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;

namespace FoundationCore.Web.Models.CustomProperties
{
    [PropertyDefinitionTypePlugIn, Serializable]
    public class PropertyLongUrl : PropertyLongString, IReferenceMap
    {
        /// <summary>
        /// Gets the specific link type for a URL.
        /// </summary>
        /// <value>The specific link type for a URL.</value>
        /// <remarks>The link editor type for a URL is 130.</remarks>
        public virtual int LinkEditorType => 130;

        private List<Guid> _permanentLinkIdList;

        /// <summary>
        /// Gets or sets the URL resolver.
        /// </summary>
        /// <value>The URL resolver.</value>
        public Injected<IUrlResolver> LinkResolver { get; set; }

        /// <summary>
        /// Gets the type of the property value, in this case <see cref="P:EPiServer.SpecializedProperties.PropertyUrl.Url" />.
        /// </summary>
        /// <value>The type of the property value.</value>
        public override Type PropertyValueType => typeof(Url);

        protected override string LongString
        {
            get
            {
                var str = base.LongString;

                return str == "#" ? string.Empty : VirtualPathResolver.Instance.ToAbsoluteOrSame(str);
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    base.LongString = value;
                    return;
                }

                var uri = CreateUri(value);

                if (LinkResolver.Service.TryToPermanent(value, out var str))
                {
                    base.LongString = str;
                    return;
                }

                ValidateUri(uri);
                base.LongString = uri.ToString();
            }
        }

        /// <summary>
        /// This is lifted from the UriUtil (EPiServer.Web) as the method is internal.
        /// </summary>
        internal static Uri CreateUri(string link)
        {
            if (string.IsNullOrEmpty(link))
            {
                return null;
            }

            if (!Uri.TryCreate(link, UriKind.RelativeOrAbsolute, out var uri))
            {
                throw new UriFormatException(string.Format(CultureInfo.InvariantCulture, "Malformed URI? Could not create Uri from link with value '{0}'.", link));
            }

            if (uri.IsAbsoluteUri)
            {
                var authority = uri.Authority;
            }

            else if (UriUtil.IsSchemeSpecified(link))
            {
                throw new UriFormatException("Malformed URI? Absolute URI (scheme specified) which is not recognized as such by System.Uri.");
            }

            return uri;
        }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        /// <remarks>To change the URL, create a new <see cref="P:EPiServer.SpecializedProperties.PropertyUrl.Url" /> instance and set the property.</remarks>
        public Url Url
        {
            get => Value as Url;
            set => Value = value;
        }

        /// <inheritdoc />
        public override object Value
        {
            get => IsNull ? null : new Url(LongString);
            set
            {
                SetPropertyValue(value, () => LongString = value.ToString());
            }
        }

        /// <summary>
        /// Sets the internal representation from what is stored in the database. "Deserialize"
        /// </summary>
        /// <param name="value">The value.</param>
        /// <remarks>Set the value to the unresolved link value.</remarks>
        public override void LoadData(object value)
        {
            base.LongString = value as string;
        }

        /// <summary>
        /// Override to avoid the base class referring to our Value property to avoid dead-lock
        /// when this happens indirectly via DataFactory.
        /// </summary>
        public override void MakeReadOnly()
        {
            if (IsReadOnly)
            {
                return;
            }
            IsModified = false;
            IsReadOnly = true;
        }

        /// <summary>
        /// Get the data representation suitable for storing to the database.
        /// </summary>
        /// <param name="properties">The properties for the current page.</param>
        /// <returns>A string representation of the value that should be saved.</returns>
        /// <remarks>Returns the unresolved link.</remarks>
        public override object SaveData(PropertyDataCollection properties)
        {
            return base.LongString;
        }

        /// <summary>
        /// Validates the URI before saving.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <remarks>In order to abort the saving, this method should throw an <see cref="T:System.Exception" />.</remarks>
        protected virtual void ValidateUri(Uri uri)
        {
        }

        public void RemapPermanentLinkReferences(IDictionary<Guid, Guid> idMap)
        {
            var str = base.LongString;

            var guid1 = PermanentLinkUtility.GetGuid(str);

            if (guid1 == Guid.Empty)
            {
                return;
            }

            if (!idMap.TryGetValue(guid1, out var guid))
            {
                return;
            }

            if (guid == Guid.Empty)
            {
                var guid2 = Guid.NewGuid();
                var guid3 = guid2;
                idMap[guid1] = guid2;
                guid = guid3;
            }

            base.LongString = PermanentLinkUtility.ChangeGuid(str, guid);
            Modified();
        }

        public IList<Guid> ReferencedPermanentLinkIds
        {
            get
            {
                if (_permanentLinkIdList != null)
                {
                    return _permanentLinkIdList;
                }

                _permanentLinkIdList = new List<Guid>(1);

                var guid = PermanentLinkUtility.GetGuid(base.LongString);

                if (guid != Guid.Empty)
                {
                    _permanentLinkIdList.Add(guid);
                }

                return _permanentLinkIdList;
            }
        }
    }
}
