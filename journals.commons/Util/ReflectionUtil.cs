using System.ComponentModel;

namespace journals.commons.Util {
    public class ReflectionUtil {

        public static object GetProperty(object baseObject, int index) {
            var prop = TypeDescriptor.GetProperties(baseObject)[index];
            return prop?.GetValue(baseObject);
        }

        public static object GetProperty(object baseObject, string propertyName) {
            var prop = PropertyDescriptor(baseObject, propertyName);
            return prop?.GetValue(baseObject);
        }


        public static PropertyDescriptor PropertyDescriptor(object baseObject, string propertyName) {
            var prop = TypeDescriptor.GetProperties(baseObject)[propertyName];
            if (prop == null) {
                prop = TypeDescriptor.GetProperties(baseObject)[propertyName.ToUpper()];
            }
            return prop;
        }
    }
}
