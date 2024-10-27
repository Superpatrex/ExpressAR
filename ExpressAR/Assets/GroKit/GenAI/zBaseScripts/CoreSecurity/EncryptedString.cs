using System.Runtime.CompilerServices;
using UnityEngine;


[assembly: InternalsVisibleTo("CoreSecurity")]
[assembly: SuppressIldasm()]
namespace CoreSecurity
{
    [CreateAssetMenu(fileName = "SO_EncryptedString", menuName = "AICore3lb/AIEncryptedString")]

    public class SO_StringEncrypted : ScriptableObject
    {
        [SerializeField]
        [System.Reflection.Obfuscation(Exclude = true, ApplyToMembers = true)]
        public string plainString;
        [System.Reflection.Obfuscation(Exclude = true, ApplyToMembers = true)]
        [SerializeField]
        private bool isEncrypted = false;
        [System.Reflection.Obfuscation(Exclude = true, ApplyToMembers = true)]
        [SerializeField]
        private string encryptedStorage;
        public const string EMPTYSTRING = "STRING_NOT_SET";
        public const string ENCRYPTED = "ENCRYPTED";
        public bool SOEncrypted
        {
            get
            {
                return isEncrypted;
            }
        }

        public string SOEncryptedStorage
        {
            get
            {
                return encryptedStorage;
            }
        }
        public string GetString
        {
            get
            {
                if (isEncrypted)
                {

                    if (encryptedStorage == EMPTYSTRING)
                    {
                        Debug.LogError("EncryptedString is set to default");
                    }
                    CoreSecInternal securitySystem = new CoreSecInternal();
                    string decryptedString = securitySystem.DecryptStringInternalKey(encryptedStorage);
                    return decryptedString;
                }
                else
                {
                    if (plainString == EMPTYSTRING)
                    {
                        Debug.LogError("UnEncryptedString is set to default");
                    }
                    Debug.LogWarning("This String is not Encrypted");
                    return plainString;
                }

            }
        }
        private void SetString(string value)
        {
            CoreSecInternal securitySystem = new CoreSecInternal();
            encryptedStorage = securitySystem.EncryptStringInternalKey(value);
            isEncrypted = true;
        }
#if UNITY_EDITOR
        public void Editor_Encrypt()
        {
            SetString(plainString);
            plainString = ENCRYPTED;
        }

        public void Editor_Decrypt()
        {
            if (isEncrypted)
            {
                plainString = GetString;
                encryptedStorage = EMPTYSTRING;
                isEncrypted = false;
            }
        }
#endif
    }

}