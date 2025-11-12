# 🧮 Mini Compilador — SVG by Numbers (C# + WPF)

**Versión:** 1.0  
**Lenguaje del compilador:** C# (.NET 8, WPF)  
**Autor:** Elian Gomez 1-21-2318 
**Materia:** Compiladores
**Docente:** Ivan Mendoza

---

## 📘 Descripción general

Este proyecto implementa un **mini compilador visual** que traduce un lenguaje propio (diseñado para la práctica) a **gráficos SVG**.  
Cada instrucción del lenguaje describe operaciones de dibujo (líneas, rectángulos, círculos, etc.) y el compilador genera el código SVG correspondiente.

El entorno está desarrollado en **C# con WPF**, mostrando:
- Tokens léxicos generados.
- Árbol sintáctico (AST).
- Tabla de símbolos.
- Código intermedio (IR / cuadruplas).
- Código final SVG.
- Renderizado visual del resultado en tiempo real.

---

## 🧠 Arquitectura del compilador

El sistema sigue las etapas clásicas de un compilador:

| Etapa | Archivo | Descripción |
|-------|----------|-------------|
| **Análisis léxico** | `Lexer.cs` | Convierte el texto fuente en una secuencia de *tokens* (palabras clave, números, identificadores). |
| **Análisis sintáctico** | `Parser.cs` | Construye el *árbol de sintaxis abstracta (AST)* usando un parser descendente recursivo. |
| **Análisis semántico** | `Semantic.cs` | Verifica consistencia (Paper antes de dibujar, variables declaradas, rango de color, etc.). |
| **Tabla de símbolos** | `SymbolTable.cs` | Almacena variables y sus valores numéricos. |
| **Generación de código intermedio** | `Ir.cs` | Convierte las instrucciones en *cuadruplas (op, arg1, arg2, arg3...)*. |
| **Generación de código final** | `CodeGenSvg.cs` | Traduce las cuadruplas a código SVG. |
| **Ejecución / Visualización** | `MainWindow.xaml` y `.cs` | Interfaz WPF que permite escribir código, compilar y ver resultados. |

---


