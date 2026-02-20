#version 330 core
out vec4 FragColor;

in vec3 FragPos;
in vec3 Normal;

uniform vec3 lightColor;
uniform vec3 lightPos;
uniform vec3 viewPos;

void main()
{
    // Stronger ambient light to brighten shadowed sides
    float ambientStrength = 0.8;
    vec3 ambient = ambientStrength * lightColor;

    // Diffuse light for depth cues
    vec3 norm = normalize(Normal);
    vec3 lightDir = normalize(lightPos - FragPos);
    float diff = max(dot(norm, lightDir), 0.0);
    float diffuseStrength = 0.5; // reduce contrast
    vec3 diffuse = diff * diffuseStrength * lightColor;

    // Combine ambient + diffuse
    vec3 color = (ambient + diffuse) * vec3(0.8, 0.5, 0.3); // cube color
    FragColor = vec4(color, 1.0);
}
