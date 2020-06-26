#version 330 core
in vec2 TexCoord;
in vec3 Normal;  
in vec3 FragPos;  

out vec4 color;

uniform sampler2D texture;
uniform vec3 objectColor;
uniform vec3 lightColor;
uniform vec3 lightPos; 
uniform vec3 viewPos;
uniform vec3 lightMode;

void main()
{
    vec3 ambient = lightMode.x * lightColor;

	vec3 norm = normalize(Normal);
    vec3 lightDir = normalize(lightPos - FragPos);
    float diff = max(dot(norm, lightDir), 0.0) * lightMode.y;
    vec3 diffuse = diff * lightColor;
	
	vec3 viewDir = normalize(-viewPos - FragPos);
	vec3 reflectDir = reflect(-lightDir, norm);  
	
	float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
	vec3 specular = lightMode.z * spec * diffuse * lightColor;  

	color = texture(texture, TexCoord) * vec4((ambient + diffuse + specular) * objectColor, 1.0f);
}