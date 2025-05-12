source "./lib.sh"

# @type { (outputDir: string, fileName: string, ...nodesDescription: string[]) => void }
function generateAst {
  local outputDir=$1
  local baseClassName=$2
  local nodesDescription=("${@:3}")

  mkdir -p $outputDir
  local filePath="${outputDir}/${baseClassName}.cs"
  touch $filePath
  clearFile $filePath

  defineDisclaimer $filePath
  appendLine $filePath

  # @type { string[] }
  local nodesClassNames
  # @type { [className: string]: string }
  declare -A splitNodesDescription

  for item in "${nodesDescription[@]}"; do
    local entries
    IFS=':' read -ra entries <<< "$item"
    splitNodesDescription["${entries[0]}"]="${entries[1]}"
  done

  nodesClassNames="${!splitNodesDescription[@]}"

  defineVisitor $filePath $baseClassName "${nodesClassNames[@]}"

  appendLine $filePath
  defineBaseNodeClass $filePath $baseClassName

  for className in "${!splitNodesDescription[@]}"; do
    local solidFields="${splitNodesDescription[$className]}"
    local fields
    IFS=',' read -ra fields <<< "$solidFields"

    appendLine $filePath
    defineNode $filePath $className $baseClassName "${fields[@]}"
  done
}

# @type { (filePath: string) => void }
function defineDisclaimer {
  local filePath=$1

  appendLine $filePath "// ##############################################"
  appendLine $filePath "// ####  This file was generated!            ####"
  appendLine $filePath "// ####  Do not change it manually           ####"
  appendLine $filePath "// ####  Check: cs-lox/tools/generateAst.sh  ####"
  appendLine $filePath "// ##############################################"
}

# @type { (filePath: string, ...nodesClassNames: string[]) => void }
function defineVisitor {
  local filePath=$1
  local baseClassName=$2
  local nodesClassNames=("${@:3}")

  appendLine $filePath "interface ${baseClassName}NodeVisitor<ReturnValue> {"

  local argumentName=$(toLowerCase $baseClassName)
  for className in ${nodesClassNames[@]}; do
    if [[ $className == *$baseClassName ]]; then
      appendLine $filePath "  ReturnValue Visit${className}(${className} ${argumentName});"
    else
      appendLine $filePath "  ReturnValue Visit${className}${baseClassName}(${className} ${argumentName});"
    fi
  done

  appendLine $filePath "}"
}

# @type { (filePath: string) => void }
function defineBaseNodeClass {
  local filePath=$1
  local baseClassName=$2

  appendLine $filePath "abstract class ${baseClassName} {"
  appendLine $filePath "  public abstract ReturnType Accept<ReturnType>(${baseClassName}NodeVisitor<ReturnType> visitor);"
  appendLine $filePath "}"
}

# @type { (filePath: string, className: string, ...fields: string[]) => void }
function defineNode {
  local filePath=$1
  local className=$2
  local baseClassName=$3
  local fields=("${@:4}")

  appendLine $filePath "class ${className}("

  local fieldsLength="${#fields[@]}"
  local iterationLimit=$(($fieldsLength - 1))
  for index in $(seq 0 $iterationLimit); do
    local trimmedField=$(trim "${fields[$index]}")

    if [[ $index -lt $iterationLimit ]]; then
      appendLine $filePath "  ${trimmedField},"
    else
      appendLine $filePath "  ${trimmedField}"
    fi
  done

  appendLine $filePath ") : ${baseClassName} {"

  for field in "${fields[@]}"; do
    local parts
    IFS=' ' read -ra parts <<< "$field"
    local type="${parts[0]}"
    local name="${parts[1]}"
    appendLine $filePath "  public readonly ${type} ${name} = ${name};"
  done

  appendLine $filePath "  public override ReturnType Accept<ReturnType>(${baseClassName}NodeVisitor<ReturnType> visitor) {"

  if [[ $className == *$baseClassName ]]; then
    appendLine $filePath "    return visitor.Visit${className}(this);"
  else
    appendLine $filePath "    return visitor.Visit${className}${baseClassName}(this);"
  fi

  appendLine $filePath "  }"

  appendLine $filePath "}"
}
