using System;

namespace BooBinding
{
	public class BooDefaultMethod : MonoDevelop.Projects.Dom.DomMethod
	{
	}
	
	public class Constructor : BooDefaultMethod
	{
		public TypeMembers()
		{
		}
	}
}
/*
class Constructor(BooDefaultMethod):
	def constructor(m as ModifierEnum, region as IRegion, bodyRegion as IRegion):
		Name = 'ctor'
		self.region = region
		self.bodyRegion = bodyRegion
		modifiers = m

class Destructor(BooDefaultMethod):
	def constructor(className as string, m as ModifierEnum, region as IRegion, bodyRegion as IRegion):
		Name = '~' + className
		self.region = region
		self.bodyRegion = bodyRegion
		modifiers = m

class BooDefaultMethod(DefaultMethod):
	[Property(Node)]
	_node as AST.Method
	
	def AddModifier(m as ModifierEnum):
		modifiers = modifiers | m

class Field(DefaultField):
	def AddModifier(m as ModifierEnum):
		modifiers = modifiers | m
	
	def constructor(rtype as IReturnType, name as string, m as ModifierEnum, region as IRegion):
		self.returnType = rtype
		self.Name = name
		self.region = region
		modifiers = m
	
	def SetModifiers(m as ModifierEnum):
		modifiers = m

class Indexer(DefaultIndexer):
	def AddModifier(m as ModifierEnum):
		modifiers = modifiers | m
	
	def constructor(rtype as IReturnType, parameters as ParameterCollection, m as ModifierEnum, region as IRegion, bodyRegion as IRegion):
		returnType = rtype
		self.parameters = parameters
		self.region = region
		self.bodyRegion = bodyRegion
		modifiers = m

class Method(BooDefaultMethod):
	def constructor(name as string, rtype as IReturnType, m as ModifierEnum, region as IRegion, bodyRegion as IRegion):
		Name = name
		self.returnType = rtype
		self.region = region
		self.bodyRegion = bodyRegion
		modifiers = m

class Property(DefaultProperty):
	[Property(Node)]
	_node as AST.Property
	
	def AddModifier(m as ModifierEnum):
		modifiers = modifiers | m
	
	def constructor(name as string, rtype as IReturnType, getter as IMethod, setter as IMethod, getRegion as IRegion, setRegion as IRegion, m as ModifierEnum, region as IRegion, bodyRegion as IRegion):
		self.Name = name
		self.returnType = rtype
		self.getterMethod = getter
		self.setterMethod = setter
		self.getterRegion = getRegion
		self.setterRegion = setRegion
		self.region = region
		self.bodyRegion = bodyRegion
		modifiers = m
*/
