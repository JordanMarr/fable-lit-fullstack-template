module Shared.Validation

type StringValue = string
type PropertyName = string
type Error = { ErrorMessage: string }
type Rule = Result<unit, string>
type PropertyRules = Map<string, Rule list>

/// Contains resulting validation errors and provides error aggregation methods.
type ValidationResult =
    {
        ErrorsByProperty: Map<PropertyName, Error list>
    }

    /// Checks if the entity (or a specific property) has errors.
    member this.HasErrors(?property: string) = 
        match property with
        | Some prop -> 
            match this.ErrorsByProperty.TryGetValue(prop) with
            | true, errors -> errors.Length > 0
            | _ -> false
        | None -> 
            this.ErrorsByProperty
            |> Map.toList
            |> List.exists (fun (prop, errors) -> errors.Length > 0)
    
    /// Returns true if entity has no errors.
    member this.IsValid() = 
        not (this.HasErrors())

    /// Gets errors for the entity (or for a specific property).
    member this.GetErrors(?property: string) = 
        this.ErrorsByProperty
        |> Map.toList
        |> List.filter (fun (prop, errors) -> match property with | Some p -> p = prop | _ -> true)
        |> List.collect (fun (_, errors) -> errors)


/// Runs all rules an populates errors.
let validate<'Entity> (propertyRules: PropertyRules) : ValidationResult = 
    { ValidationResult.ErrorsByProperty = 
        propertyRules 
        |> Seq.map (fun kvp -> 
            let propName = kvp.Key
            let rules = kvp.Value
            
            let errors = 
                rules
                |> List.choose (function | Error err -> Some err | _ -> None)
                |> List.map (fun err -> { Error.ErrorMessage = System.String.Format(err, propName) })
            
            propName, errors
        )
        |> Map.ofSeq }

/// Initializes validation results.
let noErrors = { ErrorsByProperty = Map.empty<PropertyName, Error list> }

/// Initializes property rules for an entity.
let rules : PropertyRules = Map.empty

/// Adds rules for a property to an entity validator.
let rulesFor<'Entity> (property: string) (rules: Rule list) (propertyRules: PropertyRules) = 
    propertyRules.Add(property, rules) : PropertyRules


module Rules =
    /// Requires that a given string is not null or white space.
    let required (value: string) =
        if System.String.IsNullOrWhiteSpace(value) then Error "{0} is required" else Ok()
    
    /// Requires that an obj is not null.
    let requiredObj (value: obj) =
        match value |> Option.ofObj with
        | Some _ -> Ok()
        | None -> Error "{0} is required"

    /// Asserts that a condition is true or shows given error.
    let isTrue (condition: bool) (err: string) = 
        if condition then Ok() else Error err

    /// Asserts that a condition is false or shows given error.
    let isFalse (condition: bool) (err: string) = 
        isTrue (condition = false) err
    
    /// Requires that a string is not shorter than the given min length.
    let minLen (min: int) (value: string) = if (string value).Length < min then Error (sprintf "{0} must be at least %i characters" min) else Ok()
    
    /// Requires that a string is not longer than the given max length.
    let maxLen (max: int) (value: string) = if (string value).Length > max then Error (sprintf "{0} exceeds the max length of %i" max) else Ok()

    /// Value must be Greater Than n.
    let gt n value = if value > n then Ok() else Error (sprintf "{0} must be greater than %A" n)
    
    /// Value must be Greater Than or Equal to n.
    let gte n value = if value >= n then Ok() else Error (sprintf "{0} must be greater than %A" n)
    
    /// Value must be Less Than n.
    let lt n value = if value < n then Ok() else Error (sprintf "{0} must be greater than %A" n)
    
    /// Value must be Less Than or Equal to n.
    let lte n value = if value <= n then Ok() else Error (sprintf "{0} must be greater than %A" n)

    let regex (pattern: string) (description: string) (value: string) = 
        let m = System.Text.RegularExpressions.Regex.Match(value, pattern)
        if m.Success then Ok() else Error (sprintf "{0} is not a valid %s" description)
