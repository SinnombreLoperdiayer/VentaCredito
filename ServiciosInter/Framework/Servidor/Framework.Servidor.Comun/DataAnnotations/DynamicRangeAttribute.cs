using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Framework.Servidor.Comun.DataAnnotations.Exceptions;
using System.ComponentModel.DataAnnotations;
using DataAnnotationFramework;

namespace Framework.Servidor.Comun.DataAnnotations
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
  public class DynamicRangeAttribute : DynamicValidationAttribute
  {
    public DynamicRangeAttribute(Type targetType, string message) :
      base(message)
    {
      TargetType = targetType;
    }

    //public DynamicRangeAttribute(Type targetType) :
    //  base(Resource.StandardRangeError)
    //{
    //  TargetType = targetType;
    //}

    public string SMinimum
    {
      set
      {
        if (value == null)
        {
          Minimum = null;
          return;
        }
        IComparable res;
        try
        {
          res = (value as IConvertible).ToType(TargetType, CultureInfo.InvariantCulture) as IComparable;
          Minimum = res;
        }
        catch
        {
          throw (new FormatException(Resource.InvalidFormat));
        }
      }
      get
      {
        return null;
      }
    }

    public string SMaximum
    {
      set
      {
        if (value == null)
        {
          Maximum = null;
          return;
        }
        IComparable res;
        try
        {
          res = (value as IConvertible).ToType(TargetType, CultureInfo.InvariantCulture) as IComparable;
          Maximum = res;
        }
        catch
        {
          throw (new FormatException(Resource.InvalidFormat));
        }
      }
      get
      {
        return null;
      }
    }

    private Type TargetType { get; set; }

    public IComparable Minimum { get; protected set; }

    public IComparable Maximum { get; protected set; }

    public string DynamicMaximum { get; set; }

    public string DynamicMinimum { get; set; }

    public string DynamicMaximumDelay { get; set; }

    public string DynamicMinimumDelay { get; set; }

    private void addDelay(string delayRef, ref IComparable value, ValidationContext validationContext)
    {
      if (string.IsNullOrWhiteSpace(delayRef)) return;
      PropertyAccessor delayProp = null;
      try
      {
        delayProp = new PropertyAccessor(validationContext.ObjectInstance, delayRef, false);
      }
      catch
      {
        return;
      }
      if (delayProp == null) return;
      object toAdd = delayProp.Value;
      if (toAdd == null) return;
      if (TargetType == typeof(Int32))
      {
        value = Convert.ToInt32(value) + Convert.ToInt32(toAdd);
      }
      else if (TargetType == typeof(Int16))
      {
        value = Convert.ToInt16(value) + Convert.ToInt16(toAdd);
      }
      else if (TargetType == typeof(Int64))
      {
        value = Convert.ToInt64(value) + Convert.ToInt64(toAdd);
      }
      else if (TargetType == typeof(UInt32))
      {
        value = Convert.ToUInt32(value) + Convert.ToUInt32(toAdd);
      }
      else if (TargetType == typeof(UInt16))
      {
        value = Convert.ToUInt16(value) + Convert.ToUInt16(toAdd);
      }
      else if (TargetType == typeof(UInt64))
      {
        value = Convert.ToUInt64(value) + Convert.ToUInt64(toAdd);
      }
      else if (TargetType == typeof(byte))
      {
        value = Convert.ToByte(value) + Convert.ToByte(toAdd);
      }
      else if (TargetType == typeof(sbyte))
      {
        value = Convert.ToSByte(value) + Convert.ToSByte(toAdd);
      }
      else if (TargetType == typeof(decimal))
      {
        value = Convert.ToDecimal(value) + Convert.ToDecimal(toAdd);
      }
      else if (TargetType == typeof(float))
      {
        value = Convert.ToSingle(value) + Convert.ToSingle(toAdd);
      }
      else if (TargetType == typeof(double))
      {
        value = Convert.ToDouble(value) + Convert.ToDouble(toAdd);
      }
      else if (TargetType == typeof(DateTime))
      {
        value = Convert.ToDateTime(value).Add((TimeSpan)toAdd);
      }
      return;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      DeclareDependency(DynamicMaximum, DynamicMinimum, DynamicMinimumDelay, DynamicMaximumDelay);
      if (value == null) return ValidationResultFor(null, validationContext, true);

      IComparable toCheck = value as IComparable;
      if (toCheck == null) throw (new InvalidAttributeApplicationException(Resource.InvalidDateRangeApplication));

      if (validationContext != null)
      {
        if (Minimum != null && toCheck.CompareTo(Minimum) < 0)
          return ValidationResultFor(FormatErrorMessage(validationContext.DisplayName), validationContext, false);
        if (Maximum != null && toCheck.CompareTo(Maximum) > 0)
          return ValidationResultFor(FormatErrorMessage(validationContext.DisplayName), validationContext, false);
      }
      else
      {
        if (Minimum != null && toCheck.CompareTo(Minimum) < 0)
          return ValidationResultFor(FormatErrorMessage("Value"), validationContext, false);
        if (Maximum != null && toCheck.CompareTo(Maximum) > 0)
          return ValidationResultFor(FormatErrorMessage("Value"), validationContext, false);
      }
      if (!string.IsNullOrWhiteSpace(DynamicMinimum) && validationContext != null)
      {
        PropertyAccessor dynamicMinimumProp =
            new PropertyAccessor(validationContext.ObjectInstance, DynamicMinimum, false);

        if (dynamicMinimumProp != null && dynamicMinimumProp.Value != null)
        {
          IComparable dMin = dynamicMinimumProp.Value as IComparable;

          if (dMin == null) throw (new InvalidDynamicRangeException(Resource.InvalidLowerDynamicRange));
          addDelay(DynamicMinimumDelay, ref dMin, validationContext);

          if (toCheck.CompareTo(dMin) < 0)
            return ValidationResultFor(FormatErrorMessage(validationContext.DisplayName), validationContext, false);
        }
      }
      if (!string.IsNullOrWhiteSpace(DynamicMaximum) && validationContext != null)
      {
        PropertyAccessor dynamicMaximumProp =
            new PropertyAccessor(validationContext.ObjectInstance, DynamicMaximum, false);

        if (dynamicMaximumProp != null && dynamicMaximumProp.Value != null)
        {
          IComparable dMax = dynamicMaximumProp.Value as IComparable;
          if (dMax == null) throw (new InvalidDynamicRangeException(Resource.InvalidLowerDynamicRange));

          addDelay(DynamicMaximumDelay, ref dMax, validationContext);

          if (toCheck.CompareTo(dMax) > 0)
            return ValidationResultFor(FormatErrorMessage(validationContext.DisplayName), validationContext, false);
        }
      }
      return ValidationResultFor(null, validationContext, true);
    }
  }
}