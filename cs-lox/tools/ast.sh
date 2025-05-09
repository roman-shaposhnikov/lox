# @type { (outputDir: string, fileName: string, ...nodesDescription: string[]) => void }
function generateAst {
  local outputDir=$1
  local fileName=$2
  local nodesDescription=("${@:3}")

  mkdir -p $outputDir
  local filePath="${outputDir}/${fileName}.cs"
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

  defineVisitor $filePath "${nodesClassNames[@]}"

  appendLine $filePath
  defineBaseNodeClass $filePath

  for className in "${!splitNodesDescription[@]}"; do
    local solidFields="${splitNodesDescription[$className]}"
    local fields
    IFS=',' read -ra fields <<< "$solidFields"

    appendLine $filePath
    defineNode $filePath $className "${fields[@]}"
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
  local nodesClassNames=("${@:2}")

  appendLine $filePath "interface AstVisitor<ReturnValue> {"

  for className in ${nodesClassNames[@]}; do
    appendLine $filePath "  ReturnValue Visit${className}Expression(${className} expression);"
  done

  appendLine $filePath "}"
}

# @type { (filePath: string) => void }
function defineBaseNodeClass {
  local filePath=$1

  appendLine $filePath "abstract class ${BASE_CLASS_NAME} {"
  appendLine $filePath "  public abstract ReturnType Accept<ReturnType>(AstVisitor<ReturnType> visitor);"
  appendLine $filePath "}"
}

# @type { (filePath: string, className: string, ...fields: string[]) => void }
function defineNode {
  local filePath=$1
  local className=$2
  local fields=("${@:3}")

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

  appendLine $filePath ") : ${BASE_CLASS_NAME} {"

  for field in "${fields[@]}"; do
    local parts
    IFS=' ' read -ra parts <<< "$field"
    local type="${parts[0]}"
    local name="${parts[1]}"
    appendLine $filePath "  public readonly ${type} ${name} = ${name};"
  done

  appendLine $filePath "  public override ReturnType Accept<ReturnType>(AstVisitor<ReturnType> visitor) {"
  appendLine $filePath "    return visitor.Visit${className}Expression(this);"
  appendLine $filePath "  }"

  appendLine $filePath "}"
}
